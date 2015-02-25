/**
    6 Degrees of Freedom
    [Sends both gyroscope and accelerometer data to Serial]

    CODE:
    File Name = sixDOF.ino
    Authors = Rico Tiongson, Nina Sanchez
    Convention = MicrosoftStyle + camelCase

    EQUIPMENT:
    Accelerometer = ADXL345
    Gyroscope = itg3205
    
    SERIAL SETTINGS:
    Baud Rate = [editable]
    
    ACCELEROMETER SETTINGS:
    Range = +/- (2g, 4g, 8g, or 16g)
    Calibration at Zero = [editable]
    
    GYROSCOPE SETTINGS:
    Parameter = +/- 2000 (deg/sec)
    Sensitivity = 14.375
    Calibration at Zero = [editable]
    
    DEFAULT WIRING:
    VDO => 3.3V
    GND => GND
    SDA => A4
    SCL => A5
    CLK => pin13 [editable]
    DO => pin12 [editable]
    DI => pin11 [editable]
    CS => pin10 [editable]
    INT0 => open
    INT1 => open
    
    POWER MANAGEMENT:
    Clock Select = internal oscillator
    Reset = False
    Sleep Mode = False
    Standby Mode = False
    Interrupt = False
    Sample Rate = 125Hz
    Low Pass Filter = 5Hz
**/

#include <Wire.h> // I2C library, gyroscope

// output from 6DOF
int ax, ay, az; // stores acceleration vector (idle => upward)
int gx, gy, gz, gtemp; // raw values from gyroscope
float pitch, yaw, roll, temperature; // calculated values from gyroscope (deg/s and Celcius)

// Serial Options
#define WRITE_BYTES // uncomment to write in bytes
#define INCLUDE_TEMPERATURE // uncomment to include temperature for Serial

// user settings: change when needed
const int BaudRate = 9600;
const int AccelRange = 16; // +/- 2g, 4g, 8g, or 16g
const int AccelZero[3] = {0, 0, 0}; // calibration values for when accelerometer is flat
const int GyroZero[3] = {-14, 7, -7}; // calibration values for when gyroscope is idle

// pin usage: change assignment if you want to
const int CLK = 13; // Serial comm clock
const int DO = 12; // Data Out
const int DI = 11; // Data In
const int CS = 10; // Chip Select
// connect VDD-GND to 3.3V-GND
// connect SDA-SCL to A4-A5

// start of program
void setup() {
    Serial.begin(BaudRate);
    Wire.begin();
    initAccelerometer();
    initGyroscope();
}

// loop of program
void loop() {
    getAccelerometerData();
    getGyroscopeData();
    printToSerial();
}


void printToSerial() {
    #ifndef WRITE_BYTES
      #ifdef INCLUDE_TEMPERATURE
        Serial.println(String(ax) + " " + ay + " " + az + " " + pitch + " " + yaw + " " + roll + " " + temperature);
      #else
        Serial.println(String(ax) + " " + ay + " " + az + " " + pitch + " " + yaw + " " + roll);
      #endif
    #else
      Serial.write((byte *) &ax, 2);
      Serial.write((byte *) &ay, 2);
      Serial.write((byte *) &az, 2);
      Serial.write((byte *) &pitch, 4);
      Serial.write((byte *) &yaw, 4);
      Serial.write((byte *) &roll, 4);
      #ifdef INCLUDE_TEMPERATURE
        Serial.write((byte *) &temperature, 4);
      #endif
    #endif
}

/// COMPLICATED CODES

#define GyroAddress 0x68 // (0x68 or 0x69) for when AD0 is connected to (GND or VCC)
#define SampleRateDivider 0x15
#define DigitalLowPassFilter 0x16
#define IntConfiguration 0x17
#define PowerManagement 0x3E
#define BytesToInt(buffer) (((int) (buffer)[0] << 8) | (buffer)[1])

void initGyroscope() {
    writeGyro(PowerManagement, 0x00);
    writeGyro(SampleRateDivider, 0x07); // EB, 50, 80, 7F, DE, 23, 20, FF
    writeGyro(DigitalLowPassFilter, 0x1E); // +/- 2000 deg/s, 1KHz
    writeGyro(IntConfiguration, 0x00);
}

#define DataFormat 0x31
#define PowerControl 0x2D
#define MeasureMode 0x08
#define TwoG 0x08
#define FourG 0x09
#define EightG 0x0A
#define SixteenG 0x0B

void initAccelerometer() {
    
    pinMode(CS, OUTPUT); // Chip Select = 0
    writeAccel(DataFormat);
    
    int range;
    switch (AccelRange) {
        case 16: range = SixteenG; break;
        case 8: range = EightG; break;
        case 4: range = FourG; break;
        case 2: range = TwoG; break;
        default: range = SixteenG;
    }
    
    writeAccel(range);
    
    // clock for one second
    pinMode(CS, INPUT);
    delay(1);
    pinMode(CS, OUTPUT);
    
    writeAccel(PowerControl);
    writeAccel(MeasureMode);
    pinMode(CS, INPUT);
    delay(1);
    
}

#define AccelBytes 6
#define StartAddress 0xF2

byte AccelBuffer[AccelBytes];
byte AccelData;

void getAccelerometerData() {
    
    pinMode(CS, OUTPUT);
    
    // start address is 0x32
    // D7 = 1 for read and D6 = 1 for sequential read
    writeAccel(StartAddress);
    
    for (int i = AccelBytes - 1; i >= 0; --i) {
        writeAccel(0x00);
        AccelBuffer[i] = AccelData;
    }
    
    az = BytesToInt(AccelBuffer + 0) - AccelZero[2];
    ay = BytesToInt(AccelBuffer + 2) - AccelZero[1];
    ax = BytesToInt(AccelBuffer + 4) - AccelZero[0];
    
    pinMode(CS, INPUT);
    
}

#define GyroBytes 8
#define GyroSensitivity 14.375f

byte GyroBuffer[GyroBytes];

void getGyroscopeData() {
    
    /************************************
     * Gyroscope Registers:
     * temp MSB = 1B, temp LSB = 1C
     * x axis MSB = 1D, x axis LSB = 1E
     * y axis MSB = 1F, y axis LSB = 20
     * z axis MSB = 21, z axis LSB = 22
     ************************************/
    
    int RegAddress = 0x1B;
    readGyro(RegAddress, GyroBytes, GyroBuffer);
    
    // get raw data
    
    gx = BytesToInt(GyroBuffer + 2) - GyroZero[0];
    gy = BytesToInt(GyroBuffer + 4) - GyroZero[1];
    gz = BytesToInt(GyroBuffer + 6) - GyroZero[2];
    gtemp = BytesToInt(GyroBuffer + 0);
    
    // process derived data (deg/s and Celcius)
    pitch = gx / GyroSensitivity;
    yaw = gy / GyroSensitivity;
    roll = gz / GyroSensitivity;
    temperature = 35 + ((float) (gtemp) + 13200) / 280;
    
}



// Writes data to accelerometer
void writeAccel(byte data) {
    
    AccelData = 0;
    
    for (int b = 8; b > 0; --b) {
        
        pinMode(CLK, OUTPUT);
        if (data & 0x80)
            pinMode(DI, INPUT); // if MSB = 1
        else
            pinMode(DI, OUTPUT);
        
        data <<= 1;
        pinMode(CLK, INPUT);
        
        // read
        AccelData <<= 1;
        if (digitalRead(DO) == HIGH)
            AccelData |= 1;
        
        // reset DI to 1
        pinMode(DI, INPUT);
        
    }
    
}

// Writes val to address register
void writeGyro(byte address, byte val) {
    Wire.beginTransmission(GyroAddress);
    Wire.write(address);
    Wire.write(val);
    Wire.endTransmission();
}

// Reads num bytes from address register to buffer array
void readGyro(byte address, int num, byte buffer[]) {
    Wire.beginTransmission(GyroAddress);
    Wire.write(address);
    Wire.endTransmission();
    
    Wire.beginTransmission(GyroAddress);
    Wire.requestFrom(GyroAddress, num); // request num bytes from ACC
    
    for (int i = 0; Wire.available(); ++i)
        buffer[i] = Wire.read();
    
    Wire.endTransmission();
}

