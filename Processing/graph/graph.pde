import java.util.*;
import java.nio.ByteBuffer;
import java.awt.*;
import javax.swing.*;
import processing.serial.*;

JFrame frame = new JFrame("Time graphs");
Serial serial = null;
TimeGraph[] accel = new TimeGraph[3];
TimeGraph[] gyro = new TimeGraph[3];
int[] colors = {0xFF0000, 0x00FF00, 0x0000FF};
int Baud = 9600;

int ax, ay, az;
float pitch, yaw, roll, temperature;

void setup() {
  
  if (Serial.list().length > 0) {
    serial = new Serial(this, Serial.list()[0], Baud);
  }
  
  frame.setSize(500, 400);
  frame.setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);
  frame.setResizable(true);
  frame.setLayout(new GridLayout(2, 1));
  
  for (int i = 0; i < accel.length; ++i) {
    accel[i] = new TimeGraph(5000, -400, 400);
    gyro[i] = new TimeGraph(5000, -200, 200);
    accel[i].setColor(new Color(colors[i]));
    gyro[i].setColor(new Color(colors[i]));
  }
  
  frame.add(new TimeCanvas(accel));
  frame.add(new TimeCanvas(gyro));
  frame.setVisible(true);
  // /*
  if (serial == null) {
    new java.util.Timer(true).scheduleAtFixedRate(
      new TimerTask() {
        long timeStart = System.currentTimeMillis();
        public void run() {
          float time = (System.currentTimeMillis() - timeStart) / 1000.0f;
          float scale = 350;
          accel[0].put(scale * sin(time / 2));
          accel[1].put(scale * cos(time));
          accel[2].put(abs((time % 5) * scale / 5 - scale / 2) - scale / 4);
          gyro[0].put(scale * sin(time * 13) * sin(time));
          gyro[1].put(scale * cos(time) * cos(time * 5));
          gyro[2].put(-scale * sin(time * 3) * sin(time * 2));
        }
      }, 2000, 1000 / TimeCanvas.FPS
    );
  }
  // */
}

void draw() {
  setVisible(false);
  if (serial != null && serial.available() > 22) {
        
    byte[] buffer = new byte[22];
    for (int i = 0; i < 22; ++i)
      buffer[i] = (byte) serial.read();
    
    // reverse bytes for java parsing
    for (int i = 0; i < 6; i += 2) { // shorts
      swap(buffer, i, i + 1);
    }
    for (int i = 6; i < 22; i += 4) { // floats
      swap(buffer, i, i + 3);
      swap(buffer, i + 1, i + 2);
    }
    
    ByteBuffer bb = ByteBuffer.wrap(buffer);
    ax = bb.getShort();
    ay = bb.getShort();
    az = bb.getShort();
    pitch = bb.getFloat();
    yaw = bb.getFloat();
    roll = bb.getFloat();
    temperature = bb.getFloat();
    
    accel[0].put(ax);
    accel[1].put(ay);
    accel[2].put(az);
    
    gyro[0].put(pitch);
    gyro[1].put(yaw);
    gyro[2].put(roll);
    
    System.out.println(ax + " " + ay + " " + az + " " + pitch + " " + yaw + " " + roll);
  }
}

void swap(byte[] buffer, int i, int j) {
  byte temp = buffer[i];
  buffer[i] = buffer[j];
  buffer[j] = temp;
}
