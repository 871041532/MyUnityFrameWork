# -*- coding: utf-8 -*-
import os
import sys
import socket
import _thread
import ctypes
STD_INPUT_HANDLE = -10
STD_OUTPUT_HANDLE= -11
STD_ERROR_HANDLE = -12
FOREGROUND_BLACK = 0x0
FOREGROUND_BLUE = 0x01 # text color contains blue.
FOREGROUND_GREEN= 0x02 # text color contains green.
FOREGROUND_RED = 0x04 # text color contains red.
FOREGROUND_YELLOW = 0x0e # yellow.
FOREGROUND_INTENSITY = 0x08 # text color is intensified.
BACKGROUND_BLUE = 0x10 # background color contains blue.
BACKGROUND_GREEN= 0x20 # background color contains green.
BACKGROUND_RED = 0x40 # background color contains red.
BACKGROUND_INTENSITY = 0x80 # background color is intensified.
class Color:
    ''' See http://msdn.microsoft.com/library/default.asp?url=/library/en-us/winprog/winprog/windows_api_reference.asp
    for information on Windows APIs. - www.jb51.net'''
    std_out_handle = ctypes.windll.kernel32.GetStdHandle(STD_OUTPUT_HANDLE)
    def set_cmd_color(self, color, handle=std_out_handle):
        """(color) -> bit
        Example: set_cmd_color(FOREGROUND_RED | FOREGROUND_GREEN | FOREGROUND_BLUE | FOREGROUND_INTENSITY)
        """
        bool = ctypes.windll.kernel32.SetConsoleTextAttribute(handle, color)
        return bool

    def set_red(self):
        self.set_cmd_color(FOREGROUND_RED | FOREGROUND_INTENSITY)

    def set_yellow(self):
        self.set_cmd_color(FOREGROUND_YELLOW | FOREGROUND_INTENSITY)

    def set_white(self):
        self.reset_color()

    def print(self, strs):
        print (strs)

    def print_yellow(self, strs):
        self.set_cmd_color(FOREGROUND_YELLOW | FOREGROUND_INTENSITY)
        print (strs)
        self.reset_color()

    def print_red(self, print_text):
        self.set_cmd_color(FOREGROUND_RED | FOREGROUND_INTENSITY)
        print (print_text)
        self.reset_color()


    def reset_color(self):
        self.set_cmd_color(FOREGROUND_RED | FOREGROUND_GREEN | FOREGROUND_BLUE)


    def print_green(self, print_text):
        self.set_cmd_color(FOREGROUND_GREEN | FOREGROUND_INTENSITY)
        print (print_text)
        self.reset_color()
   
    def print_blue_text(self, print_text):
        self.set_cmd_color(FOREGROUND_BLUE | FOREGROUND_INTENSITY)
        print (print_text)
        self.reset_color()
   
    def print_red_text_with_blue_bg(self, print_text):
        self.set_cmd_color(FOREGROUND_RED | FOREGROUND_INTENSITY| BACKGROUND_BLUE | BACKGROUND_INTENSITY)
        print (print_text)
        self.reset_color()

# 获取端口号
try:
    args = sys.argv[1:]
    port = int(args[0])
except Exception as e:
    print("请输入端口号！")


# GM相关指令
class LogType:
    Error = "#0"
    Assert = "#1"
    Warning = "#2"
    Log = "#3"
    Exception = "#4"


# GM处理函数
color_log = Color()

def switch_error():
    color_log.set_red()
    color_log.print("\nTrace or Error:")

def switch_warning():
    color_log.set_yellow()
    color_log.print("\nWarning:")

def switch_normal():
    color_log.print("")
    color_log.set_white()


gm_dict = {
    LogType.Error: switch_error,
    LogType.Assert: switch_error,
    LogType.Warning: switch_warning,
    LogType.Log: switch_normal,
    LogType.Exception: switch_error,
}

# 解析发来的字符串
def prase_str(strs):
    # if strs == "Unity已断开服务器。":
        # os.system('cls')
    if strs in gm_dict:
        gm_dict[strs]()
    else:
        color_log.print(strs)

# 启动UDP服务端
s = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
s.bind(('', port))
color_log.print_green("UPD Server Open, Port: " + args[0])
is_init = False
addr = 0

def receive_input():
    while True:
        strs = input()
        s.sendto(strs.encode("utf-8"), addr)

while True:
    data, client_addr = s.recvfrom(8192)
    addr = client_addr
    prase_str(data.decode(encoding="utf-8"))
    if not is_init:
        is_init = True
        _thread.start_new_thread(receive_input, ())