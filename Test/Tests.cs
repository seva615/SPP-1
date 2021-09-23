using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Trace;
using Xunit;

namespace Tests{
    public class Foo{
        private ITracer _tracer;

        public Foo(ITracer tracer){
            _tracer = tracer;
        }

        public void M1(){
            _tracer.StartTrace();
            Thread.Sleep(100);
            _tracer.StopTrace();
        }

        public void M2(){
            _tracer.StartTrace();
            Thread.Sleep(200);
            _tracer.StopTrace();
        }

        public void M3(){
            M1();
            M2();
        }

        public void M4(){
            _tracer.StartTrace();
            M1();
            _tracer.StopTrace();
        }
    }
    class Method{
        string ClassName { get; }
        string MethodName { get; }
        Method[] Children { get; }

        public Method(string className, string methodName, params Method[] children){
            ClassName = className;
            MethodName = methodName;
            Children = children;
        }

        void Check(IMethod node){
            Assert.Equal(ClassName, node.ClassName);
            Assert.Equal(MethodName, node.MethodName);
            var enumerator = Children.GetEnumerator();
            foreach (var actualMethod in node.Methods){
                Assert.True(enumerator.MoveNext());
                ((Method)enumerator.Current).Check(actualMethod);
            }
        }

        public static void Check(Method[] expected, IEnumerable<IMethod> actual){
            var enumerator = expected.GetEnumerator();
            foreach (var actualMethod in actual){
                Assert.True(enumerator.MoveNext());
                ((Method)enumerator.Current).Check(actualMethod);
            }
        }
    }

    public class Tests{
        [Fact]
        public void TestTracerInSingleThread(){
            var tracer = new Tracer();
            var foo = new Foo(tracer);
            foo.M3();
            Method[] expectedMethods = new Method[]{
                new Method("Foo", "M1"),
                new Method("Foo", "M2"),
            };

            IEnumerable<IThreadNode> result = tracer.GetResult();
            IEnumerator<IThreadNode> nodes = result.GetEnumerator();
            Assert.True(nodes.MoveNext());
            IThreadNode node = nodes.Current;
            Assert.False(nodes.MoveNext()); 
            Assert.Equal(node.Thread, Thread.CurrentThread);
            Method.Check(expectedMethods, node.Methods);
        }

        [Fact]
        public void TestTracerInMultyThread(){
            var tracer = new Tracer();
            var foo = new Foo(tracer);
            var m1 = new Method("Foo", "M1");
            var m2 = new Method("Foo", "M2");
            var task = Task.WhenAll(
                Task.Run(() =>{
                    foo.M3();
                    return (Thread.CurrentThread, new Method[] { m1, m2 });
                }),
                Task.Run(() =>{
                    foo.M4();
                    return (Thread.CurrentThread, new Method[] { new Method("Foo", "M4", m1, m2) });
                }),
                Task.Run(() =>{
                    foo.M2();
                    return (Thread.CurrentThread, new Method[] { m2 });
                }));

            task.Wait();
            var expectedResult = task.Result;
            var actualResult = tracer.GetResult();
            foreach (var pair in expectedResult){
                IThreadNode node = null;
                foreach (var n in actualResult){
                    if (Equals(pair.Item1, n.Thread)){
                        node = n;
                        break;
                    }
                }
                Assert.NotNull(node);
                Method.Check(pair.Item2, node.Methods);
            }
        }
    }
}
