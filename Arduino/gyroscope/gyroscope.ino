//From http://www.varesano.net//
#include <Wire.h> // I2C library, gyroscope
// Gyroscope ITG3200 
#define GYRO 0x68 //  when AD0 is connected to GND ,gyro address is 0x68.
//#define GYRO 0x69   when AD0 is connected to VCC ,gyro address is 0x69  
#define G_SMPLRT_DIV 0x15
#define G_DLPF_FS 0x16
#define G_INT_CFG 0x17
#define G_PWR_MGM 0x3E
#define G_TO_READ 8 // 2 bytes for each axis x, y, z
// offsets are chip specific. 
int g_offx = 120;
int g_offy = 20;
int g_offz = 93;
//initializes the gyroscope
void initGyro()
{
  /*****************************************
   * ITG 3200
   * power management set to:
   * clock select = internal oscillator
   * no reset, no sleep mode
   * no standby mode
   * sample rate to = 125Hz
   * parameter to +/- 2000 degrees/sec
   * low pass filter = 5Hz
   * no interrupt
   ******************************************/
  writeTo(GYRO, G_PWR_MGM, 0x00);
  writeTo(GYRO, G_SMPLRT_DIV, 0x07); // EB, 50, 80, 7F, DE, 23, 20, FF
  writeTo(GYRO, G_DLPF_FS, 0x1E); // +/- 2000 dgrs/sec, 1KHz, 1E, 19
  writeTo(GYRO, G_INT_CFG, 0x00);
}
void getGyroscopeData(int * result)
{
  /**************************************
   * Gyro ITG-3200 I2C
   * registers:
   * temp MSB = 1B, temp LSB = 1C
   * x axis MSB = 1D, x axis LSB = 1E
   * y axis MSB = 1F, y axis LSB = 20
   * z axis MSB = 21, z axis LSB = 22
   *************************************/
  int regAddress = 0x1B;
  int temp, x, y, z;
  byte buff[G_TO_READ];
  readFrom(GYRO, regAddress, G_TO_READ, buff); //read the gyro data from the ITG3200
  result[0] = ((buff[2] << 8) | buff[3]) + g_offx;
  result[1] = ((buff[4] << 8) | buff[5]) + g_offy;
  result[2] = ((buff[6] << 8) | buff[7]) + g_offz;
  result[3] = (buff[0] << 8) | buff[1]; // temperature
}
//

float startTime;
float currentTime, deltaTime;

#define TIME ((float) millis() / 1000)
#define TOTAL_TIME (TIME - startTime)
#define print3(x,y,z) (Serial.println(String(x) + " " + y + " " + z))

void setup()
{
  Serial.begin(9600);
  Wire.begin();
  initGyro();
  currentTime = startTime = TIME;
}

double Ex = 0, Ey = 0, Ez = 0;
double totalTime = 0;
// calibration for gyro at rest
float zeroX = -60;
float zeroY = 58;
float zeroZ = -140;

void loop()
{
  float time = TIME;
  deltaTime = time - currentTime;
  currentTime = time;

  byte addr;
  int gyro[4];
  getGyroscopeData(gyro);
  float sensitivity = 14.375f;
  
  // degrees per second
  float gx = (gyro[0] - zeroX) / sensitivity;
  float gy = (gyro[1] - zeroY) / sensitivity;
  float gz = (gyro[2] - zeroZ) / sensitivity;
  
  double temperature = 35+ ((double) (gyro[3] + 13200)) / 280; // celcius
  
  Ex += gx * deltaTime;
  Ey += gy * deltaTime;
  Ez += gz * deltaTime;
  
  // print3(gyro[0], gyro[1], gyro[2]);
  
   print3(Ex, Ey, Ez);
  /*Serial.print(" F=");
   Serial.print(turetemp);
   Serial.print(' ');
   Serial.println("C");*/
  // delay(1000);
}
//---------------- Functions
//Writes val to address register on ACC
void writeTo(int DEVICE, byte address, byte val) {
  Wire.beginTransmission(DEVICE); //start transmission to ACC 
  Wire.write(address);        // send register address
  Wire.write(val);        // send value to write
  Wire.endTransmission(); //end transmission
}
//reads num bytes starting from address register on ACC in to buff array
void readFrom(int DEVICE, byte address, int num, byte buff[]) {
  Wire.beginTransmission(DEVICE); //start transmission to ACC 
  Wire.write(address);        //sends address to read from
  Wire.endTransmission(); //end transmission

    Wire.beginTransmission(DEVICE); //start transmission to ACC
  Wire.requestFrom(DEVICE, num);    // request 6 bytes from ACC

  int i = 0;
  while(Wire.available())    //ACC may send less than requested (abnormal)
  { 
    buff[i] = Wire.read(); // receive a byte
    i++;
  }
  Wire.endTransmission(); //end transmission
}

