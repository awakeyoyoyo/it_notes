## Redis事务操作

Redis事务的主要作用就是串联多个命令，防止其它命令插队，串行执行，

#### Multi Exec Discard

`Multi`：组队阶段，提交任务执行顺序

`Exec`：执行命令

`Discard`： 放弃组队

#### 各个阶段错误处理：

- 组队阶段 ：不会报错
- 执行阶段 ：除了报错，其它全部执行成功

#### Redis事务冲突问题

修改值之前需要判断值大小，随后多个线程同时修改。

#### Redis

`watch key` 如果事务之前这个key被其它线程命令修改过，那么事务被打断。

`unwatch`

Redis事务三特性

- 单独的隔离操作 ：所有的命令都会序列化按照顺序执行，事务执行过程不回被其它客户端发来的命令打断。
- 没有隔离级别的概念：队列中命令没有提交之前不会被执行。
- 不保证原子性： 如果事务中有命令执行失败，其它命令还会继续执行，不会回滚

### 秒杀事务

> 自写可能有问题

```java
 @Test
    public void skill(){
        if (jedis.getGoodsNums()>0){
            while(jedis.geGoodstNums()>0) {
                jedis.watch(goodsNums);
                jedis.mutli();
                if (jedis.incre(goodsNums)>=0) {
                    //秒杀成功
                    break;
                }
            }
        }
    }
```