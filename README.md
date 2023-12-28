# Identity Server System (IDS)
- ระบบยืนยันตัวตนและกำหนดสิทธิ์การใช้งานผ่านเครือข่ายกลาง
- เว็บแอปพลิเคชันที่ทําหน้าที่เป็นตัวกลางให้กับผู้ใช้งานในการเข้าใช้งานซอฟต์แวร์ที่แตกต่างกัน โดยใช้ชื่อผู้ใช้งาน (Username) และรหัสผ่าน (Password) เพียงชุดเดียว รวมถึงกําหนดสิทธิ์การใช้งานให้กับผู้ใช้งานในการเข้าใช้งานซอฟต์แวร์
  
# Architecture
![alt text](https://github.com/TiTle162/Identity-Server-System/blob/main/IDS-Architecture.PNG?raw=true)

# Screens
## 1. Admin login page.
![alt text](https://github.com/TiTle162/Identity-Server-System/blob/main/IDS-Screens/Admin%20Login%20Page.PNG?raw=true)
## 2. User register page.
![alt text](https://github.com/TiTle162/Identity-Server-System/blob/main/IDS-Screens/User%20Register%20Page.PNG?raw=true)
## 3. Application register page.
![alt text](https://github.com/TiTle162/Identity-Server-System/blob/main/IDS-Screens/Client%20Login%20Page.PNG?raw=true)
## 4. Mapping user and application page.
![alt text](https://github.com/TiTle162/Identity-Server-System/blob/main/IDS-Screens/Mapping%20User%20And%20Client%20Page.PNG?raw=true)
## 5. Login to application via IDS system (OAuth 2.0) page.
![alt text](https://github.com/TiTle162/Identity-Server-System/blob/main/IDS-Screens/OAuth%20Login%20Page.png?raw=true)

# โครงสร้างโฟลเดอร์
```
-> Identity-Server-System
    -> Basics    โฟลเดอร์พื้นฐานการ Authentication และ Authorization โดยใช้             
    -> Server    โฟลเดอร์ setup OAuth 2.0 และส่วนของหน้า login ผ่านระบบ
    
    * โฟลเดอร์อื่นๆ คือแอปพลิเคชันสําหรับใช้ในการทดสอบการ redirect มายังหน้า login ระบบ รวมถึงการ config แอปพลิเคชันให้สามารถทํางานร่วมกับระบบ 
Identity Server System
    ** ส่วนของเว็บไซต์สําหรับ Management ผู้ใช้งานและแอปพลิเคชัน (Front-End) ไม่สามารถนํามาลง GitHub ได้ เนื่องจากติดลิขสิทธิ์ 
```
## เริ่มต้น
- ติดตั้ง Visual Studio ก่อนทําการ Clone โปรเจค
- ติดตั้ง Microsoft SQL Server

## Clone Project
ใช้คําสั่ง `git clone https://github.com/TiTle162/Identity-Server-System.git` เพื่อโคลนโปรเจค 

## Start Project
1. เลือกรันโปรเจคชื่อ IdentityServer 
2. เลือกรันโปรเจคอื่นๆ เช่น ApiOne สําหรับใช้ทดสอบการ redirect มายังหน้า login ของระบบ
3. ตรวจสอบ Microsoft SQL Server ว่ามี Database ที่เกี่ยวข้องกับ Entity Framework Core สําหรับ Authenticatoin และ IdentityServer4 สําหรับ Authorization
4. เพิ่มข้อมูล user ตาม [เอกสาร](https://identityserver4.readthedocs.io/en/latest/quickstarts/5_entityframework.html)
5. ในกรณีที่เลือก ApiOne ให้ไปที่ url สําหรับเข้าใช้งานระบบ ApiOne (.../secret)
6. ระบบ ApiOne จะทําการ redirect มายังหน้า login ของระบบ Identity Server System
7. กรอก username และ password ของ user
8. หากข้อมูลถูกต้อง ระบบ Identity Server System จะ redirect กลับไปยังระบบ ApiOne ที่ url ในขั้นตอนที่ 5

## ปล.
แต่ละโปรเจคที่ถูกสร้างขึ้นจากเครื่องมือหรือเทคโนโลยีที่แตกต่างกัน มักจะมีการ config ระบบให้สามารถใช้งานกับระบบ Identity Server System ที่แตกต่างกัน ดังนั้น ควรศึกษาวิธีการ config ระบบเพิ่มเติม

