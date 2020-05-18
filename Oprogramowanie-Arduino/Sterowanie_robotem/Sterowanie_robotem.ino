#include <SoftwareSerial.h>
#include "ardprintf.h"
SoftwareSerial RFID(2, 3); // RX, TX

#define PWM_L_PIN 5
#define PWM_R_PIN 6

#define MOTOR_L_PIN1 9
#define MOTOR_L_PIN2 10

#define MOTOR_R_PIN1 7
#define MOTOR_R_PIN2 8

#define DISTANCE_SENSOR_PIN A3
#define VOLTAGE_SENSOR_PIN A2
#define GROUND_SENSOR1_PIN A0
#define GROUND_SENSOR2_PIN A1

#define HEADER "X"
#define CRC_9 0x31

byte CRC8;
char pDataFrame[50];
char buf[50];

void setup() {
  Serial.begin(9600);
  RFID.begin(9600);
  pinMode(MOTOR_L_PIN1, OUTPUT);
  pinMode(MOTOR_L_PIN2, OUTPUT);
  pinMode(MOTOR_R_PIN1, OUTPUT);
  pinMode(MOTOR_R_PIN2, OUTPUT);

  digitalWrite(MOTOR_L_PIN1, LOW);
  digitalWrite(MOTOR_L_PIN2, LOW);
  digitalWrite(MOTOR_R_PIN1, LOW);
  digitalWrite(MOTOR_R_PIN2, LOW);

  analogWrite(PWM_L_PIN, 0);
  analogWrite(PWM_R_PIN, 0);
  analogReference(INTERNAL);
  delay(2000);
}

void go_straight(byte PWM_Speed){
  analogWrite(PWM_L_PIN, PWM_Speed);
  analogWrite(PWM_R_PIN, PWM_Speed);

  digitalWrite(MOTOR_L_PIN1, LOW);
  digitalWrite(MOTOR_L_PIN2, HIGH);
  digitalWrite(MOTOR_R_PIN1, LOW);
  digitalWrite(MOTOR_R_PIN2, HIGH);
}

void go_back(byte PWM_Speed){
  analogWrite(PWM_L_PIN, PWM_Speed);
  analogWrite(PWM_R_PIN, PWM_Speed);

  digitalWrite(MOTOR_L_PIN1, HIGH);
  digitalWrite(MOTOR_L_PIN2, LOW);
  digitalWrite(MOTOR_R_PIN1, HIGH);
  digitalWrite(MOTOR_R_PIN2, LOW);
}

void go_stop(byte PWM_Speed){
  analogWrite(PWM_L_PIN, PWM_Speed);
  analogWrite(PWM_R_PIN, PWM_Speed);

  digitalWrite(MOTOR_L_PIN1, LOW);
  digitalWrite(MOTOR_L_PIN2, LOW);
  digitalWrite(MOTOR_R_PIN1, LOW);
  digitalWrite(MOTOR_R_PIN2, LOW);
}

int get_distance(){
  return analogRead(DISTANCE_SENSOR_PIN);
}

void get_distance_ground(int &distance1, int &distance2){
  distance1 = analogRead(GROUND_SENSOR1_PIN);
  distance2 = analogRead(GROUND_SENSOR2_PIN);
}

double get_voltage(){
  return analogRead(VOLTAGE_SENSOR_PIN)*(1.1/1024.0)*7.5; // dla dzielnika 47k i 6.8k max napiecie 8,7V
}

bool check_base(){
  byte inByte[17];
  RFID.readBytes(inByte, 17);
  if(inByte[0]== '\n' && inByte[1]== '\r' && inByte[2]=='S' && inByte[3]=='N' && inByte[4]==' ' && inByte[5]=='0' && inByte[6]=='x'  && inByte[7]=='0'  && inByte[8]=='3'  && inByte[9]=='C'  && inByte[10]=='6' && inByte[11]=='2' && inByte[12]=='7' && inByte[13]=='0' && inByte[14]=='0'  && inByte[15]=='3'  && inByte[16]=='2' ){
    for(int i = 0; i < 17; i++ ) inByte[i] = 0;
    return 1;
  }
  return 0;
}

byte CRC8_DataArray(byte *pData, byte Len){
    byte CRC_final = 0xff;

    for(int i = 0; i < Len; ++i){
        CRC_final = CRC8_SingleByte(CRC_final, pData[i]);
    }
return CRC_final;
}

byte CRC8_SingleByte(byte CRC_prev, byte Data){
    CRC_prev ^= Data;
    for(int i = 0; i < 8; ++i){
        if((CRC_prev & 0x80) != 0){
            CRC_prev = (byte)((CRC_prev << 1) ^ CRC_9);}
        else{
            CRC_prev = (byte)(CRC_prev << 1);}
    }
    return CRC_prev;
}

String readCRC(String pData){
    pData.substring(0,pData.length()-9).toCharArray(buf, 50);
    byte t_CRC8 = CRC8_DataArray((unsigned char*)buf, strlen(buf));
    //Serial.print(pData);  
    //Serial.println(buf);
    if(String((int)t_CRC8, HEX) == pData.substring(pData.length()-8,pData.length()-6) && pData.substring(0,1) == String(HEADER)){
      return pData.substring(2,pData.length()-9);
    }
    else return String("Incorrect CRC");
}

void loop() {
  int distL, distR;
  get_distance_ground(distL, distR);

  Serial.print("lewy czujnik podloza: ");
  Serial.print(distL);
  Serial.print(" prawy czujnik podloza: ");
  Serial.print(distR);
  Serial.print(" czujnik odleglosci: ");
  Serial.print(get_distance());
  Serial.print(" napiecie baterii: ");
  Serial.print(get_voltage());
  Serial.print(" wykrycie bazy: ");
  Serial.println(check_base());
  delay(200);
  
//  distL > 950 || distR > 950 ||get_distance() > 100 ||
//  if(  check_base() ==1 ){
//     go_stop(150);
//  }else{
//    go_straight(150);
//  }

/***KOMUNIKACJA Z RPI*****/
  sprintf(pDataFrame, "%s %d %d", HEADER, 100, -200); //string do wyslania
  CRC8 = CRC8_DataArray((unsigned char*)pDataFrame, strlen(pDataFrame)); //obliczanie crc8
  ardprintf("%s %x \r\n", (char*)pDataFrame, (int)CRC8); //wysylanie po uart
  Serial.println(readCRC(Serial.readString())); //odczyt danych ze stringu w argumencie 
  
}
