import socket

s = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
# 指定 IP:port
s.bind(('192.168.1.5', 9000))
print('\033[0;32;40m欢迎使用学生选课系统\033[0m')
print('\033[1;31m')
print("UPD Server Open, Port: 9000")
while True:
 data, addr = s.recvfrom(1024)
 print(data.decode(encoding="utf-8"))