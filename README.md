# XkyPlatform
侠客云开放平台SDK_DEMO

## 下载和运行
https://github.com/xksoft/xky/releases/latest



## 如何编译
  
项目使用`VS2019`开发，.net版本使用`4.6.1`版本，纯原生`wpf`项目，没有引入第三方ui库，第一次加载项目请先还原nuget依赖库(`右键解决方案-还原nuget包`)，然后下载ffmpeg库解压释放到程序目录（Debug或Release）下即可运行。

ffmpeg https://static.xky.com/download/ffmbeg_libs.rar

## 项目说明

### Xky.Core
核心库项目，封装了大部分API操作和socket连接对象

### Xky.Platform
项目主程序，由wpf开发，所有界面和控件都在这个项目中，包含模块的运行控制管理等

### Xky.Socket
Socket连接库

## Copyright and license

Apache License 2.0
