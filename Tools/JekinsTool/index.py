# -*- encoding: utf-8 -*-
import  subprocess
import os
import sys
import time
# subprocess.check_call("svn cleanup --remove-unversioned")
# subprocess.check_call("svn revert . -R")
# subprocess.check_call("svn update")

# Unity安装目录
# unity_exe = r'"D:\Unity\2019.1.8f1\Editor\Unity.exe"'
unity_exe = r'"C:\Program Files\Unity\Hub\Editor\2019.1.8f1\Editor\Unity.exe"'
# unity工程目录
project_path = os.getcwd()
# 日志
log_file = os.getcwd() + '/Tools/JekinsTool/log.txt'

def clear_log():
	f = open(log_file, "w+")
	f.truncate()
	f.close()

def kill_unity():
	os.system('taskkill /IM Unity.exe /F')

def call_unity_static_func(func):
	kill_unity()
	time.sleep(1)
	clear_log()
	time.sleep(1)
	print('call unity static func: ' + func)
	subprocess.check_call(unity_exe + ' -projectPath ' + project_path + ' -quit -logFile ' + log_file + ' -executeMethod ' + func)

def debug_log():
	f = open(log_file, "rb")
	# line = f.read()
	# line =
	lines = f.readlines()
	for line in lines:
		print(line)
	f.close()
	

if __name__ == '__main__':
	call_unity_static_func('MyBuildApp.BuildIos')
	debug_log()
	print('done')