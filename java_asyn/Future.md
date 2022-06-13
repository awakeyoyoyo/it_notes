## JAVA Future机制

### Future

> Futrue是个接口。Future就是对于具体的Runnable或者Callable任务的执行结果进行取消、查询是否完成、获取结果。必要时可以通过get方法获取执行结果，该方法会阻塞直到任务返回结果。

### FutureTask

FutureTask是Future的具体实现。`FutureTask`实现了`RunnableFuture`接口。`RunnableFuture`接口又同时继承了`Runnable` 和 `Runnable` 接口。所以`FutureTask`既可以作为Runnable被线程执行，又可以作为Future得到Callable的返回值。

> FutureTask的作用就是既保留了Callable的返回值特性，又拥有future的控制任务执行的方法，又可以 当成Runnable直接被线程执行。

### FutureTask实现方式

#### 构造函数

```java
public FutureTask(Callable<V> callable) {
    if (callable == null)
        throw new NullPointerException();
    this.callable = callable;
    this.state = NEW;       // ensure visibility of callable
}

public FutureTask(Runnable runnable, V result) {
    this.callable = Executors.callable(runnable, result);
    this.state = NEW;       // ensure visibility of callable
}
```

这两个构造函数区别在于，如果使用第一个构造函数最后获取线程实行结果就是callable的执行的返回结果；而如果使用第二个构造函数那么最后获取线程实行结果就是参数中的result，接下来让我们看一下FutureTask的run方法。

#### Run方法

> 底层执行callable()得到返回值

```java
public void run() {
        //如果状态不是new，或者runner旧值不为null(已经启动过了)，就结束
        if (state != NEW ||
            !UNSAFE.compareAndSwapObject(this, runnerOffset,
                                         null, Thread.currentThread()))
            return;
        try {
            Callable<V> c = callable; // 这里的callable是从构造方法里面传人的
            if (c != null && state == NEW) {
                V result;
                boolean ran;
                try {
                    result = c.call(); //执行任务，并将结果保存在result字段里。
                    ran = true;
                } catch (Throwable ex) {
                    result = null;
                    ran = false;
                    setException(ex); // 保存call方法抛出的异常
                }
                if (ran)
                    set(result); // 保存call方法的执行结果
            }
        } finally {
            // runner must be non-null until state is settled to
            // prevent concurrent calls to run()
            runner = null;
            // state must be re-read after nulling runner to prevent
            // leaked interrupts
            int s = state;
            if (s >= INTERRUPTING)
                handlePossibleCancellationInterrupt(s);
        }
    }
```