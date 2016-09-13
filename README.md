## RedisCleaner
This small project was created due to redis limitations, that can't clean up expired objects fast enough.
What it does, is running SCAN command with supplied bulk size in -b parameter value on pattern in -p parameter

Project written using .NET Core

To install .NET Core visit http://dot.net and install the one for your OS.

#### Building
1. Clone repo
2. Ubuntu run `build.sh`
3. Windows run `build.cmd`
4. run `app\RedisCleaner` will output help screen

```
Parameters:
-c <redis connection string>     -Redis full connection string
-p <pattern>     -Redis KEYS command pattern
-b <bulk size>       -Number of items to process in batch
Usage Example:
RedisCleaner.exe -c "myredishost:6379,password=*****,abortConnect=False" -p "cache:*" -b 100
```
