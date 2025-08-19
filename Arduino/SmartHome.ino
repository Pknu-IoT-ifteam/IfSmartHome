#include <DHT.h>
#include <Servo.h>
#include <SDL_Arduino_INA3221.h>

SDL_Arduino_INA3221 ina3221;

#define INA 4
#define INB 3

#define DHTPIN 7 
#define DHTTYPE DHT11
DHT dht(DHTPIN, DHTTYPE);

#define MOTORPIN 6

#define RED_PIN   9
#define GREEN_PIN 10
#define BLUE_PIN  11

#define RELAY_PIN_TEM 8
#define RELAY_PIN_MOTOR 5
#define RELAY_PIN_LED 12
#define RELAY_PIN_PAN 13

#define CHANNEL_PAN 1
#define CHANNEL_LED 2
#define CHANNEL_MOTOR 3

bool DHTOn = false;
bool motorOn = false;
bool LEDOn = false;
bool PANOn = false;

int tem = 0;
int hum = 0;

Servo myservo;

void setup() {
  Serial.begin(9600);

  dht.begin();

  pinMode(RELAY_PIN_TEM, OUTPUT);
  pinMode(RELAY_PIN_MOTOR, OUTPUT);
  pinMode(RELAY_PIN_LED, OUTPUT);
  pinMode(RELAY_PIN_PAN, OUTPUT);

  pinMode(RED_PIN, OUTPUT);
  pinMode(GREEN_PIN, OUTPUT);
  pinMode(BLUE_PIN, OUTPUT);

  pinMode(INA, OUTPUT);
  pinMode(INB, OUTPUT);

  digitalWrite(RELAY_PIN_TEM, LOW);  // 초기값
  digitalWrite(RELAY_PIN_MOTOR, LOW); // ON
  digitalWrite(RELAY_PIN_LED, LOW);

  analogWrite(RED_PIN, 255);
  analogWrite(GREEN_PIN, 255);
  analogWrite(BLUE_PIN, 255);

  ina3221.begin();
}

void dht11() {    //함수 dht11선언
  tem = dht.readTemperature();    //변수 t에 온도 값을 저장
  hum = dht.readHumidity();   //변수 h에 습도 값을 저장
  Serial.print("Temperature : ");   //문자열 출력
  Serial.print(tem);    //변수 t출력
  Serial.println("C");    //문자열 출력
  Serial.print("Humidity : ");    //문자열 출력
  Serial.print(hum);    //변수 h출력
  Serial.println("%");    //문자열 출력
}

void relayControlBySerial() {
  if (Serial.available()) {
    char cmd = Serial.read();
    if (cmd == '1') {
      digitalWrite(RELAY_PIN_TEM, LOW);   // 릴레이 OFF
      DHTOn = false;
      Serial.println("RelayTem OFF, 온습도 측정 중지");
    } 
    else if (cmd == '2') {
      digitalWrite(RELAY_PIN_TEM, HIGH);  // 릴레이 ON
      DHTOn = true;
      Serial.println("RelayTem ON, 온습도 측정 시작");
    } 
    else if (cmd == '3') {
      digitalWrite(RELAY_PIN_MOTOR, LOW);   // 릴레이 OFF
      motorOn = false;
      Serial.println("RelayMotor OFF, 모터 중지");
    } 
    else if (cmd == '4') {
      digitalWrite(RELAY_PIN_MOTOR, HIGH);  // 릴레이 ON
      myservo.attach(6);      // 서보 활성화
      motorOn = true;
      Serial.println("RelayMotor ON, 모터 시작");
    }
    else if (cmd == '5') {
      digitalWrite(RELAY_PIN_LED, LOW);   // 릴레이 OFF
      LEDOn = false;
      Serial.println("RelayLED OFF, LED 중지");
    } 
    else if (cmd == '6') {
      digitalWrite(RELAY_PIN_LED, HIGH);  // 릴레이 ON
      analogWrite(RED_PIN, 0);
      analogWrite(GREEN_PIN, 0);
      analogWrite(BLUE_PIN, 0);
      LEDOn = true;
      Serial.println("RelayLED ON, LED 시작");
    }
    else if (cmd == '7') {
      digitalWrite(RELAY_PIN_PAN, LOW);   // 릴레이 OFF
      PANOn = false;
      // digitalWrite(INA, LOW);
      // digitalWrite(INB, LOW);

      Serial.println("RelayPAN OFF, PAN 중지");
    }
    else if (cmd == '8') {
      digitalWrite(RELAY_PIN_PAN, HIGH);   // 릴레이 ON
      PANOn = true;
      digitalWrite(INA, HIGH);
      digitalWrite(INB, LOW);
      Serial.println("RelayPAN ON, PAN 시작");
    }
    else if (cmd == 'q'){
      Serial.end();
    }
  }
}

void sweepMotor(){
  for (int pos = 0; pos <= 100; pos += 10) {
    myservo.write(pos);    
    delay(15);  
  }
  for (int pos = 100; pos >= 0; pos -= 10) { 
    myservo.write(pos); 
    delay(15); 
  }
}

void readIna3221(int ch){
  float busvoltage1 = 0; // 버스 전압(V, 즉 배터리 등 실제 측정하려는 전압)
  float shuntvoltage1 = 0;  // 센서의 쇼트(분압저항) 양단 전압(mV)
  float loadvoltage1 = 0; // 실제 부하(Load)에 걸리는 전압(V)
  float current_mA1 = 0; // 전류(mA)를 저장

  busvoltage1 = ina3221.getBusVoltage_V(ch);
  shuntvoltage1 = ina3221.getShuntVoltage_mV(ch);
  current_mA1 = -ina3221.getCurrent_mA(ch);
  loadvoltage1 = busvoltage1 + (shuntvoltage1 / 1000);
  
  Serial.print(String(ch) + " Bus Voltage:   "); Serial.print(busvoltage1); Serial.println(" V");
  Serial.print(String(ch) + " Shunt Voltage: "); Serial.print(shuntvoltage1); Serial.println(" mV");
  Serial.print(String(ch) + " Load Voltage:  "); Serial.print(loadvoltage1); Serial.println(" V");
  Serial.print(String(ch) + " Current 1:       "); Serial.print(current_mA1); Serial.println(" mA");
  Serial.println("");
}

void loop() {
  relayControlBySerial(); // 항상 시리얼 명령 체크
  
  if (DHTOn) {
    dht11();  // 릴레이가 ON일 때만 측정 및 출력
    delay(2000); // 2초 딜레이
  } 

  if(motorOn){
    Serial.println("모터 회전 명령 보냄");
    sweepMotor();
    readIna3221(CHANNEL_MOTOR);
    delay(2000);
  }
  else{ 
    // readIna3221(CHANNEL_MOTOR); 
    // delay(2000);
  }

  if(LEDOn){
    readIna3221(CHANNEL_LED);
    delay(2000);
  } 
  else{ 
    // readIna3221(CHANNEL_LED); 
    // delay(2000);
  }

  if(PANOn){
    readIna3221(CHANNEL_PAN);
    delay(2000);
  } 
  else{ 
    // readIna3221(CHANNEL_PAN); 
    // delay(2000);
  }

}