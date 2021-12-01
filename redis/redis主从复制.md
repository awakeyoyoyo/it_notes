## Redis 主从复制

### 含义

主机数据更新后根据配置和策略，自动同步到备机的机制master/slaver机制，Master以写为主，Slave以读为主。

### 好处

- 读写分离->主服负责写，从服负责读
- 容灾快速恢复->从服挂了，可以快速从主服获取数据来恢复 。   主服挂了->XX



### Redis主从复制 配置过程

#### 一主两从

命令：

`info replication` 打印主从关系

从服配置：

slaveof <ip><port>   //成为ip:port上Reids的从服

从机不能写只能读取

