# TracertX

Tracert command with geographic address, with cz88 database and GeoIP2 database inside. 

带地理位置的Tracert命令，内置了纯真IP数据库和GeoIP2数据库。

通过本地查询的方式返回Tracert路径上各个节点的地理位置。

# Usage

	TracertX.exe [-w Timeout=5000] [-i TTL=64] [-d Searcher=0] <Target>

## Example1

Use default database to tracert www.microsoft.com:

	TracertX.exe www.microsoft.com
	
## Output1

	Ping 210.192.117.42 Success.

	Tracert:
	1       192.168.0.1     局域网 对方和您在同一内部网
	2       192.168.1.1     局域网 对方和您在同一内部网
	3       59.173.128.9    湖北省武汉市 电信
	4       111.175.208.53  湖北省武汉市 电信
	5       111.175.208.225 湖北省武汉市 电信
	6       202.97.78.141   中国 电信骨干网
	7       218.30.19.106   陕西省西安市 电信骨干网
	10      210.192.117.42  北京市 万网公司IDC

## Example2

Use GeoIP2 database to tracert www.amazon.com: 

	TracertX.exe -d 1 www.amazon.com
	
## Output2

	Ping 104.71.152.212 Success.

	Tracert:
	1       192.168.0.1     Unknown
	2       192.168.1.1     Unknown
	3       59.173.128.1    中国湖北省武汉
	4       111.175.210.53  中国湖北省武汉
	5       111.175.210.221 中国湖北省武汉
	6       202.97.67.97    中国
	7       202.97.94.237   中国
	8       202.97.35.102   中国
	9       202.97.6.74     中国
	10      203.215.236.26  香港Central and Western DistrictCentral
	11      104.71.152.212  美国Massachusetts剑桥

# Language

cz88 database supports Chinese simplified only; GeoIP2 database prefers the current Windows language.

纯真IP数据库仅支持简体中文；GeoIP2数据库优先采用Windows系统当前语言。

