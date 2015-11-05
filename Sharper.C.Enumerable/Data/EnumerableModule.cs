using System;
using System.Collections.Generic;
using System.Linq;
using Sharper.C.Control;

namespace Sharper.C.Data
{

using static FunctionModule;
using static TrampolineModule;

public static class EnumerableModule
{
    public static IEnumerable<A> Seq<A>(params A[] items)
    =>
        items;

    public static IEnumerable<A> Seq1<A>(Func<A> fa)
    {
        yield return fa();
    }

    public static IEnumerable<A> Cons<A>(A x, IEnumerable<A> xs)
    =>
        Seq(x).Concat(xs);

    public static IEnumerable<A> Cons<A>(Func<A> x, IEnumerable<A> xs)
    =>
        Seq1(x).Concat(xs);

    public static IEnumerable<A> Enumerate<A>(IEnumerator<A> e)
    {
        while (e.MoveNext())
        {
            yield return e.Current;
        }
    }

    public static Func<IEnumerable<A>, B> MatchCons<A, B>
      ( Func<B> nil
      , Func<A, IEnumerable<A>, B> cons
      )
    =>
        xs =>
        {
            var e = xs.GetEnumerator();
            return e.MoveNext()
              ? cons(e.Current, Enumerate(e))
              : nil();
        };

    public static IEnumerable<A> Forever<A>(A a)
    {
        while (true)
        {
            yield return a;
        }
    }

    public static Func<A, IEnumerable<A>> Iterate<A>(Func<A, A> next)
    =>
        a => Iterate(next, a);

    public static IEnumerable<int> Naturals
    =>
        Iterate((int a) => a + 1)(0);

    public static IEnumerable<A> Flatten<A>(IEnumerable<IEnumerable<A>> xs)
    =>
        xs.SelectMany(Id);

    public static Func<B, Func<IEnumerable<A>, B>>FoldLeft<A, B>
      ( Func<B, A, B> f
      )
    =>
        z => e => e.Aggregate(z, f);

    public static Func<B, Func<IEnumerable<A>, B>> FoldRight<A, B>
      ( Func<A, B, B> f
      )
    =>
        z => e => e.Reverse().Aggregate(z, Flip(f));

    public static Func<B, Func<IEnumerable<A>, Trampoline<B>>>
    LazyFoldRight<A, B>
      ( Func<A, Trampoline<B>, Trampoline<B>> f
      )
    =>
        z => e => LazyFoldRight(e.GetEnumerator(), z, f);

    private static IEnumerable<A> Iterate<A>(Func<A, A> next, A a)
    {
        while (true)
        {
            yield return a;
            a = next(a);
        }
    }

    private static Trampoline<B> LazyFoldRight<A, B>
      ( IEnumerator<A> e
      , B x
      , Func<A, Trampoline<B>, Trampoline<B>> f
      )
    =>
        e.MoveNext()
        ? f(e.Current, Suspend(() => LazyFoldRight(e, x, f)))
        : Done(x);
}

}
