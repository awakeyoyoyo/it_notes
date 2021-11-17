## Redis

默认提供16个数据库，默认使用0号数据库

### Redis基础

单线程+多路IO复用技术

### 五大数据类型

#### Redis 键 key

`keys *` 查看当前库所有key

`exists key` 判断key是否存在

`del key` 删除指定的key数据

`unlink key` 根据value选择非阻塞删除，只将keys葱keyspace元数据中删除，真正的删除会在后续异步操作

`expire key 10` 10s过期时间

`ttl key` 查看还有多少秒过，-1永不过期 -2已过期

`select` 命令切换数据库

`dbsize` 查看当前数据库key 的数量

`flushdb` 清除当前库

`fulshall` 清除全部库

#### Redis 字符串String

> 底层是简单的动态字符串，类似于java的ArrayList，自动扩容。最大长度512M

`set k1 lqhao` 设置字符串

`get k1` 获取键对应值

`setnx k1 lqhao` 当key不存在对应value的时候才进行设置

`incr k1` 自增

`decr k1` 自减

`incrby k1 10` 自定义增长步长

`decrby k1 10`自定义增长步长

> 上述增加减少的操作都是原子性操作！！！

`mset k1 v1 k2 v2` 多个设置

`mget k1 k2` 多个获取

`msetnx k1 v1 k2 v2` 多个根据是否存在设置

`setex k1 v1 10` 设置值的时候设置过期时间

> 以上操作亦是原子性!!

#### Redis 集合List

> 底层实现是双向链表，可以随心所欲的高效率的插入删除任意位置的元素

`lpush/rpush k1 lqhao zpwen`  分别是头插法插入、尾插法插入袁术

`lpop/rpop k1` 分别从左边右边抛出一个值

`rpoplpush k1 k2` 从k1的右边取出值，放入k2的左边

`lrange k1 0 -1` 根据索引获取下表的元素，从左到右，0~-1代表获取所有

`llen` 获取列表的长度

`linsert <key> before <value> <newValue>` 在value的后面插入newValue 改成after也生效

`lrem <key> <n> <value>` 从左边删除n个value 从左到右

`lset <key> <index> <value>` 将列表key下标为index的值替换成value

**底层数据结构**

快速链表quickList，在列表元素较少的情况下使用一块连续的内存存储，这个结构是zipList，依旧是压缩列表，即多个元素合并在一起

它讲所有元素紧挨着一起存储，分配连续的内存。

当数据量比较大的时候改成quickList

数据量比较大的时候才改成quickList,因为普通链表需要附加的指针空间太大，会浪费空间，加上额外的两个指针。

多个压缩链表组成一个双向链表，减少每个元素就加两个指针

Redis将链表和zipList结合成quickList，将多个zipList使用双向指针串联起来，即可快速插入删除，又不会出现太大的空间冗余。

#### Redis 集合Set

> Set可以自动去重，String类型中国的无需集合，底层就是一个value为null的hash表，所有的添加删除查找都是O1

`sadd k1 lqhao zpwen` 放set中放入值

`smembers k1` 查看set的值

`smembers k1 lqhao`  检查set中是否存在lqhao

`scard k1` 返回该集合的元素个数

`srem k1 lqhao zpwen` 删除集合中的元素

`spop k1`  随机取出集合中的值

`sinter k1 k2` 返回两个元素众的交集

`sunion k1 k2` 返回两个集合中的并集

`sdiff k1 k2` 返回两个集合的差集，元素k1中不包含k2的

**redis-set的数据结构**

set的数据结构是dict字段，字段是用哈希表实现。

#### Redis 哈希

Redis hash是一个键值对集合

Redis hash是一个string类型的field和value的映射表，hash特别适合用于存储对象。

类似于java里面的Map<String,Objcet>

`hset <key> <filed> <value>` 给key集合中的 field键赋值value  可以理解为套娃

`hget <key> <filed>` 从key集合中<field>取出value

`hmset <key1><filed1><value1> <key2>...` 批量设置hash值

`hexists <key1><filed1>` 查看哈希表key中，给定的域field是否存在

`hkeys <key>` 列出该hash集合的所有field

`hvals <key>` 列出该hash集合的所有value

**hash的数据结构**

Hash类型对应的数据结构是两种：zipList 压缩列表，hashTable哈希表。当field-value长度较短且个数较少时，使用zipList，否则使用hashTable

#### Redis 有序集合 Zset

> Redis有序集合Zset和普通set相似，都是没有重复元素的字符串集合
>
> 不同的是每个成员都关联了一个评分（score），这个频繁用来按照最低分到最高分的方式排序集合中的成员，集合的成员是唯一的，但是评分可以重复
>
> 可按照顺序来获取集合

`zadd <key> <score1><value1>  <key2>...`将一个或者多个member元素机器score值存储有序集合key当中

`zrange <key><start><stop>[WITHSCORES]` 返回有序集key中，下表在start~stop之间的元素

`zrangebyscore key min max [WITHSCORES] ` 返回min和max之间的成员，有序集和成员按照score值递增从小到大排序

`zrevrangebyscore key maxmin [WITHSCORES][limit offset count] ` 同上，改为从大到小排序，查询出来

`zincrby key  addNums value` 给value增加scores+addNums

`zrem key value` 删除指定元素

`zcount key min max` 统计min到max范围内元素个数

`zrank key value`  检测出value的排名 

**数据结构**

Zset是Redis提供的特殊数据结构，底层使用了hash和跳跃表来实现。

1）hash，hash的作用用于关联元素value和权重score，保障元素value的唯一性，可以通过元素value找到相应的score值

2）跳跃表，目的是在于给元素value排序，根据score的范围获取元素列表

#### 配置表

`bine ` 绑定某个ip，指定某个ip可以访问吧

`protected-mode no/yes`   是否开启保护模式 即是否只能本地访问

`tcp-backlog `,bakclog队列总和=未完成三次握手+已完成三次握手队列，高并发下可能需要设置高的backlog来避免慢客户端连接问题

`timeout 0`  在连接死亡后 关闭连接所等待的时间

`tcp-keeplive 300` 超过300s没做操作就设定连接死亡

`pidfile` 设置文件存储进程号

#### Redis发布与订阅

>消息通信模式：发送者pub发送消息，订阅者sub接收消息。

`SUNSCRIBE channel01` 订阅频道

`public channel01 hello` 往channel01中发消息

#### Redis新数据类型

#### bitMaps

