import sys
import socket
from Color import Color

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

gm_list = [LogType.Error, LogType.Assert, LogType.Warning, LogType.Log, LogType.Exception]
gm_set = set(gm_list)
color_log = Color()

# 解析发来的字符串
def prase_str(strs):
	if strs in gm_set:
		if strs in (LogType.Error, LogType.Assert, LogType.Exception):
			color_log.set_red()
			color_log.print("\nTrace or Error:")
		elif strs == LogType.Warning:
			color_log.set_yellow()
			color_log.print("\nWarning:")
		elif strs == LogType.Log:
			color_log.print("")
			color_log.set_white()
	else:
		color_log.print(strs)

# 启动UDP客户端
s = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
s.bind(('127.0.0.1', port))
color_log.print_green("UPD Server Open, Port: " + args[0])
while True:
 data, addr = s.recvfrom(1024)
 prase_str(data.decode(encoding="utf-8"))