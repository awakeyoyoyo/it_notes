### Item07:Eliminate expired object references 消除过期的对象引用



JAVA中有自动的回收功能，但某些地方如果书写的代码不好仍然会出现内存泄漏。这里举个例子：栈实现

```java
public class MemoryLeakStack {
    private Object[] elements;
    private int size;
    private static final int DEFAULT_INITIAL_CAPACITY=16;

    public MemoryLeakStack(){
        elements=new Object[DEFAULT_INITIAL_CAPACITY];
    }

    public void push(Object e){
        ensureCapacity();
        elements[size++]=e;
    }

    public Object pop(){
        if (size==0){
            throw new EmptyStackException();
        }
        return elements[--size];
    }

    private void ensureCapacity() {
        if (elements.length==size){
            //返回一个新的数组对象，复制旧数组且扩大长度
            elements= Arrays.copyOf(elements,2*size+1);
        }
    }

}
```

一眼看去，貌似没啥问题，但是其实在pop方法中存在着安全隐患，它只是仅仅将stack的长度size--，而并没有将数组中的对象引用消除，导致会存在一直无法使用的对象存活在内存中。

这里的改进方式是：

```java
  public Object pop(){
        if (size==0){
            throw new EmptyStackException();
        }
        Object result= elements[--size];
    		elements[size]=null; //消除引用
    		return result;
    }
```

虽然看起来很简单且java也提供了自动回收机制，但日常开发中还是要注意消除引用。

那么JAVA中有哪些场景需要注意的呢？

##### 1、只要类是自己管理内存，就需要警惕。例如刚刚举的Stack例子

##### 2、对象存放到缓存。

需要注意的是当对象引用放入缓存中，如果它很久不会被使用，那么该引用应该进行消除。解决方法：可以使用WeakHashMap代表缓存。

#### **WeakHashMap细说**

和[HashMap](http://www.cnblogs.com/skywang12345/p/3310835.html)一样，WeakHashMap 也是一个**散列表**，它存储的内容也是**键值对(key-value)映射**，而且**键和值都可以是null**。
  不过WeakHashMap的**键是“弱键”**。在 WeakHashMap 中，当某个键不再正常使用时，会被从WeakHashMap中被自动移除。某个键被移除时，它对应的键值对也就从映射中有效地移除了。
  这个“弱键”的原理呢？大致上就是，**通过WeakReference和ReferenceQueue实现的**。

**WeakHashMap类的重要内容：**

```java
public class WeakHashMap<K,V>
    extends AbstractMap<K,V>
    implements Map<K,V> {
    //...
    Entry<K,V>[] table;
    private final ReferenceQueue<Object> queue = new ReferenceQueue<>();
}
```

可以看出WeakHashMap是使用一个Enrty数组存储数据。

还有一个用于存储被gc回收后的弱引用的队列ReferenceQueue

**WeakHashMap中的Enrty：**

```java
private static class Entry<K,V> extends WeakReference<Object> implements Map.Entry<K,V> {
}
```

可以看出其继承了WeakReference类。代表着存储的Entry数组是一个软引用数组

**到底是如何利用这个WeakReference以及ReferenceQueue呢？**

WeakHashMap代码片段:

```java
/**
 * Expunges stale entries from the table.
 删除旧entry
 */
private void expungeStaleEntries() {
    for (Object x; (x = queue.poll()) != null; ) {
        synchronized (queue) {
            @SuppressWarnings("unchecked")
          			//获取ReferenceQueue队列中元素的引用。即已经被gc清除的健值对
                Entry<K,V> e = (Entry<K,V>) x;
            int i = indexFor(e.hash, table.length);

            Entry<K,V> prev = table[i];
            Entry<K,V> p = prev;
            while (p != null) {
                Entry<K,V> next = p.next;
              	//找到相同
                if (p == e) {
                  	//链表删除操作
                    if (prev == e)
                        table[i] = next;
                    else
                        prev.next = next;
                    // Must not null out e.next;
                    // stale entries may be in use by a HashIterator
                    //将value的引用设置为null
                    e.value = null; // Help GC
                    size--;
                    break;
                }
                prev = p;
                p = next;
            }
        }
    }
}
```

具体的操作就是GC每次清理掉一个对象之后，引用对象会被放到ReferenceQueue中。expungeStaleEntries方法：遍历ReferenceQueue队列，将Enrty数组中对应的Entry删除，并且将其value设置为null加速Gc的过程。减少对value的引用，方便其后续被回收。

-  新建WeakHashMap，将“**键值对**”添加到WeakHashMap中。
        实际上，WeakHashMap是通过数组table保存Entry(键值对)；每一个Entry实际上是一个单向链表，即Entry是键值对链表。
-  当**某“弱键”不再被其它对象引用**，并**被GC回收**时。在GC回收该“弱键”时，**这个“弱键”也同时会被添加到ReferenceQueue(queue)队列**中。
-  当我们每次需要操作WeakHashMap时，会先同步table和queue。table中保存了全部的键值对，而queue中保存被GC回收的键值对；同步它们，就是**删除table中被GC回收的键值对**。

```java
public class ReferenceQueueTest {
    public static void main(String[] args) throws InterruptedException {
        int _1M = 1024 * 1024;

        ReferenceQueue<Object> referenceQueue = new ReferenceQueue<>();
        Thread thread = new Thread(() -> {
            try {
                int cnt = 0;
                WeakReference<byte[]> k;
                while ((k = (WeakReference) referenceQueue.remove()) != null) {
                    System.out.println((cnt++) + "回收了:" + k);
                }
            } catch (InterruptedException e) {
                // 结束循环
            }
        });
        //设置为守护线程，，当系统中全部都是守护线程的时候就会退出
        thread.setDaemon(true);
        thread.start();

        Object value = new Object();
        Map<Object, Object> map = new HashMap<>();
        for (int i = 0; i < 100; i++) {
            byte[] bytes = new byte[_1M];
            WeakReference<byte[]> weakReference = new WeakReference<byte[]>(bytes, referenceQueue);
            map.put(weakReference, value);
        }
        System.out.println("map.size->" + map.size());
    }
}
```

##### 3、监听器以及其他回调。