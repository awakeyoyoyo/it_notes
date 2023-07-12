## Zookeeper

### 搭建Zookeeper服务器

#### zoo.cfg配置文件说明

```
# zookeeper时间配置中的基本单位 (毫秒)
tickTime=2000

# 允许follower初始化连接到leader最大时长，它表示tickTime时间倍数 即:initLimit*tickTimeinitlimit=10
# 允许follower与leader数据同步最大前长，它表示tickTime时间倍数
syneLimit=5

#zookeper 数据存储目录及日志保存目录 (如果没有指明dataLogDir，则日志也保存在这个文件中
dataDir=/usr/local/zookeeper

#对客户端提供的端口号
clientPort=2181

#单个客户端与zookeeper最大并发连接数
maxClientCnxns=60

#保存的数据快照数量，之外的将会被清除
autopurge.snapRetainCount=3

#自动触发清除任务时间间隔，小时为单位。默认为0，表示不自动清除.
autopurge.purgeInterva1=1
```

### Zookeeper服务器的常用操作命令

- 重命名conf中的文件zoo_sample.cfg->zoo.cfg

- 启动zk服务器:
  
  > ./bin/zkServer.sh start ./conf/zoo.cfg

- 查看zk服务器状态:
  
  > ./bin/zkServer.sh status ./conf/zoo.cfq

- 停止zk服务器:
  
  > ./bin/zkServer.sh stop ./conf/zoo.cfg

- 连接zk服务器
  
  > ./bin/zkCli.sh

### Zookeeper内部的数据模型

- zk是如何存储数据的

zk的数据是保存在节点上，节点就是znode节点，多个znode之间构成一棵树的机构目录，顶层根目录是/

Znode的引用方式是路径引用，类似于文件路径。
Znode包含了四个部分

- data：保存数据

- acl：权限，定义了什么样的用户可以操作这个节点，能够进行什么操作
  
  - c:create 创建权限
  
  - w:write更新权限
  
  - r:read读取权限
  
  - d:delete删除权限
  
  - a:admin管理员权限-进行权限设置

- stat：描述当前znode的元数据

- child：当前节点的子节点

#### Zk中节点znode的类型

- 持久节点:创建出的节点，在会话结束后依然存在。保存数据
  
  > create 

- 持久序号节点:创建出的节点，根据先后顺序，会在节点之后带上一个数值，越后执行数值越大，适用于分布式锁的应用场景·单调递增
  
  > create -s

- 临时节点:
  临时节点是在会话结束后，自动被删除的，通过这个特性，**zk作为注册中心的时候：可以实现服务注册与发现的效果**。那么临时节点是如何维持心跳呢?
  
  - 连接zookeeper服务器后会维持会话，每个会话会有个会话id，当会话中断的时候会删除该会话id创建的所有临时节点
  
  > create -e

- 临时序号节点:跟持久序号节点相同，适用于临时的分布式锁
  
  > create -e -s

- Container节点(3.5.3版本新增): ontainer容器节点，当容器中没有任何子节点，该容器节点会被zk定期删除(60s)。

- TTL节点:可以指定节点的到期时间，到期后被k定时删除。只能通过系统配置 zookeeper,extendedTypesEnabled=true开启

#### zookeeper的持久化

zk的数据是运行在内存中，zk提供了两种持化机制:

- 事务日志
  zk把执行的命令以日志形式保存在dataLogDir指定的路径中的文件中(如果没有指定dataLogDir，则按dataDir指定的路径)。

- 数据快照
  zk会在一定的时间间隔内做一次内存数据的快照，把该时刻的内存数据保存在快照文件中。
  
  

  zk通过两种形式的持久化，在恢复时先恢复快照文件中的数据到内存中，再用日志文件中的数据做增量恢复，这样的恢复速度更快。

#### 查询节点

- 普通查询  
  
  > get  /node

- 查询节点相信信息
  
  > get -s /node
  
  - cZxid:创建节点的事务ID
  
  - mZxid:修改节点的事务ID
  
  - pZxid:添加和删除子节点的事务ID
  
  - ctime:节点创建的时间
  
  - mtime: 节点最近修改的时间
  
  - dataVersion:节点内数据的版本，每更新一次数据，版本会+1
  
  - acIVersion:此节点的权限版本
  
  - ephemeralOwner: 如果当前节点是临时节点，该值是当前节点所有者的session id。如果节点不是临时节点，则该值为零
  
  - dataLength: 节点内数据的长度
  
  - numChildren:该节点的子节点个数

#### 删除节点

- 普通删除
  
  > delete /node

- 乐观锁删除
  
  > delete -v [version] /node
  
  根据数据版本号dataVersion来进行删除，对应上才可以删除。 

#### 权限设置

- 注册当前会话的账号和密码:
  
  > addauth digest xiaowang:123456

- 创建节点并设置权限
  
  > create /node abcd auth:xiaowang:123456:cdwra
  > 
  > create [node] [value] auth:[user]:[pwd]:[auth]

- 在另一个会话中必须先使用账号密码，才能拥有操作该节点的权限

#### Curator客户端使用

##### Curator
