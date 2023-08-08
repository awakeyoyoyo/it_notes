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
  > create [node] [value] auth:[user]:[pwd]:[auth]

- 在另一个会话中必须先使用账号密码，才能拥有操作该节点的权限

#### Curator客户端使用

```java
package com.awake.zookeeper;

import com.awake.net.config.model.NetConfig;
import com.awake.net.config.model.ZookeeperRegistryProperties;
import org.apache.curator.framework.CuratorFramework;
import org.apache.curator.framework.CuratorFrameworkFactory;
import org.apache.curator.retry.RetryNTimes;
import org.apache.zookeeper.CreateMode;
import org.junit.Test;
import org.springframework.boot.autoconfigure.data.redis.RedisProperties;
import org.springframework.context.annotation.AnnotationConfigApplicationContext;

import java.io.IOException;


/**
 * @version : 1.0
 * @ClassName: zookeeperTest
 * @Description: TODO
 * @Auther: awake
 * @Date: 2023/7/11 18:16
 **/
public class CuratorTest {

    private final ZookeeperRegistryProperties zookeeperRegistryProperties = new ZookeeperRegistryProperties();

    private CuratorFramework initCuratorFramework(String s) {
        zookeeperRegistryProperties.setRetryCount(5);
        zookeeperRegistryProperties.setSessionTimeoutMs(60000);
        zookeeperRegistryProperties.setElapsedTimeMs(5000);
        zookeeperRegistryProperties.setConnectionTimeoutMs(5000);
        zookeeperRegistryProperties.setConnectionAddress(s);

        CuratorFramework curatorFramework = CuratorFrameworkFactory.newClient(
                zookeeperRegistryProperties.getConnectionAddress(),
                zookeeperRegistryProperties.getSessionTimeoutMs(),
                zookeeperRegistryProperties.getConnectionTimeoutMs(),
                new RetryNTimes(zookeeperRegistryProperties.getRetryCount(), zookeeperRegistryProperties.getElapsedTimeMs())
        );
        curatorFramework.start();
        return curatorFramework;
    }

    @Test
    public void testCurator() throws IOException {
        CuratorFramework curatorFramework = initCuratorFramework("127.0.0.1:2181");
        System.out.println(curatorFramework.getState());
        System.in.read();
    }

    @Test
    public void testCreateNode() throws Exception {
        CuratorFramework curatorFramework = initCuratorFramework("127.0.0.1:2181");
        //添加持久节点
        String path = curatorFramework.create().forPath("/curator-node");
        //添加临时序号节点
        String path2 = curatorFramework.create().withMode(CreateMode.EPHEMERAL_SEQUENTIAL).forPath("/curator-node", "just lose it".getBytes());

        System.out.println(String.format("curator create node :%s successful.", path));

        System.out.println(String.format("curator create ephemeral node :%s successful.", path2));

        System.in.read();
    }

    @Test
    public void testGetData() throws Exception {
        CuratorFramework curatorFramework = initCuratorFramework("127.0.0.1:2181");

        byte[] bytes = curatorFramework.getData().forPath("/curator-node");
        System.out.println("curator-node message:" + new String(bytes));
    }


    @Test
    public void testSetData() throws Exception {
        CuratorFramework curatorFramework = initCuratorFramework("127.0.0.1:2181");

        curatorFramework.setData().forPath("/curator-node", "just lose it".getBytes());
        byte[] bytes = curatorFramework.getData().forPath("/curator-node");
        System.out.println("curator-node message:" + new String(bytes));
    }

    @Test
    public void testCreateWithParent() throws Exception {
        CuratorFramework curatorFramework = initCuratorFramework("127.0.0.1:2181");

        String pathWithParent = "/node-parent/sub-node-1";
        String path = curatorFramework.create().creatingParentsIfNeeded().forPath(pathWithParent);
        System.out.println(String.format("curator create node :%s successfully", path));
    }

    @Test
    public void testDelete() throws Exception {
        CuratorFramework curatorFramework = initCuratorFramework("127.0.0.1:2181");

        String pathWithParent = "/node-parent";

        curatorFramework.delete().guaranteed().deletingChildrenIfNeeded().forPath(pathWithParent);
    }

    @Test
    public void testAddNodeListener() throws Exception {
        CuratorFramework curatorFramework = initCuratorFramework("127.0.0.1:2181");
    }

}

```

#### zookeeper实现分布式锁

##### zookeeper锁的种类

- 读锁

- 写锁

##### zookeeper如何上读锁    -还是个公平的

- **创建一个节点**代表公共资源

- 创建一个子**临时序号节点**，节点数据是**read**，表示读锁

- 获取当前公共资源的最小子节点。

- 判断最小节点是否是读锁
  
  - 如果不是读锁，则上锁失败，为最小节点设置监听，阻塞等待，zookeeper的监听机制会当最小节点发生变化时通知当前节点，于是执行第二步的流程
  
  - 如果是读锁则上锁成功

##### zookeeper如何上写锁

- 创建一个节点代表公共资源

- 获取改节点的所有子节点

- 判断自己是否**是最小的子节点**
  
  - 如果是最小节点，则上锁成功
  
  - 如果不是，说明前面还有锁，则上锁失败，监听最小节点，如果最小节点有变化，则回到第二步

##### 羊群效应

zookeeper最好调整为链式监听。即每次监听的是最小的节点，由上序号的节点来推动。既实现了公平锁又减少了全部都监听同一个节点

#### zookeeper的watch机制

#####  1、watch机制介绍

watch是注册在特定Znode上的触发器。当着Znode发生变化，**create，delete，setData**方法的时候，将会出发Znode上注册的对应事件，设置了Watch的客户端会收到异步通知。

客户端使用NIO通信方式监听服务端的调用

#### 2、zookeeper客户端使用watch

> get -w /node   一次性监听节点
> 
> ls -w /node 监听目录，创建和删除子节点都会收到通知。子节点新增节点不会收到通知
> 
> ls -R -w /node 监听节点中子节点的子节点的变化

#### Zookeeper分布式锁实现

互斥锁设计的核心思想：同一时间，仅一个进程/线程可以占有

1. 临时节点：利用**临时节点**，会话中断，就会删除的特点，避免死锁
2. 节点的顺序性：利用同一路径下，不能存在相同节点，节点创建存在顺序，先创建的节点的序号更小，序号最小的节点占有锁
3. Watch机制：监听当前占用锁的路径，如果锁对应的路径被修改，就唤醒所有等待的节点

#### zookeeper集群

主要角色：Leader、Follower、Observer

- Leader：主节点
  
  > 数据的读写，处理集群中所有事务请求。

- Follower：从节点
  
  > 负责数据的读，并且可以参加leader的选举

- Observer：观察者
  
  > 只负责读，不负责参加Leader的选举

#### ZAB协议

#### CAP理论

- C 一致性（Consistency）
  
  所有更新操作成功返回客户端后，所有节点在同一时间的数据完全一致

- A 可用性（Acailability）

        指服务一直可用，并且是正常影响时间

- P 分区容错性 （Partition tolerance）
  
  遇到节点故障的时候，仍然可以对外提供满足一致性或者可用性的服务
