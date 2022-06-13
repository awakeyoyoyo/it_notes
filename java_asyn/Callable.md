## Callable接口

> Runnable是执行工作的独立任务，不会返回任何值，如果我们希望任务完成之后可以有返回值，则可以实现Callable接口。Callable接口是一个具有类型参数的泛型，重写其call方法即刻，并且调用任务必须使用ExecutorService.sumbit()方法进行。

### Callable接口介绍

```java
@FunctionalInterface
public interface Callable<V> {
    V call() throws Exception;
}
```

在JDK1.8中只有一个方法的接口为函数式接口，函数式接口可以使用@FunctionalInterface注解修饰，也可以不使用

> #### 什么是函数式接口？
>
> （1）只包含一个抽象方法的接口，称为**函数式接口**。
>
> （2）你可以通过Lambda表达式来创建该接口的对象。（若Lambda表达式抛出一个受检异常，那么该异常需要在目标接口的抽象方法上进行声明）
>
> （3）我们可以在任意函数式接口上使用@FunctionalInterface注解，这样做可以检查它是否是一个函数式接口，同时javadoc也会包含一条声明，说明这个接口是一个函数式接口。

### 例子

```java
public class CallableTest {
    public static void main(String[] args) throws ExecutionException, InterruptedException {
        ExecutorService executorService= Executors.newCachedThreadPool();
        Future<String> future=executorService.submit(TaskWithResult.valueOf(1));
        System.out.println(future.get());
    }

    public static class TaskWithResult implements Callable<String> {
        private int id;

        public static TaskWithResult valueOf(int id) {
            TaskWithResult task = new TaskWithResult();
            task.id = id;
            return task;
        }

        @Override
        public String call() throws Exception {
            return "result of TaskWithResult "+id;
        }
    }
}
```

Futrue可以监视目标线程调用call的情况，当你调用Future的get()方法以获得结果时，当前线程就开始阻塞，直接call方法结束返回结果。