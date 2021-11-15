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