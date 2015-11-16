using System;
using System.Collections.Generic;
using System.Linq;

namespace Sharper.C.Data
{

using static FunctionModule;
using static Control.StepModule;

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

    public static IEnumerable<A> ToEnumerable<A>(this IEnumerator<A> e)
    {
        while (e.MoveNext())
        {
            yield return e.Current;
        }
    }

    public static B MatchCons<A, B>
      ( this IEnumerable<A> xs
      , Func<B> nil
      , Func<A, IEnumerable<A>, B> cons
      )
    {
        var e = xs.GetEnumerator();
        return e.MoveNext()
          ? cons(e.Current, ToEnumerable(e))
          : nil();
    }

    public static IEnumerable<A> Forever<A>(A a)
    {
        while (true)
        {
            yield return a;
        }
    }

    public static IEnumerable<A> Iterate<A>(A a, Func<A, A> next)
    =>
        Iterate(a, next);

    public static IEnumerable<int> Naturals
    =>
        Iterate(0, (int a) => a + 1);

    public static IEnumerable<A> Flatten<A>(IEnumerable<IEnumerable<A>> xs)
    =>
        xs.SelectMany(Id);

    public static B FoldMap<A, B>
      ( this IEnumerable<A> xs
      , B zero
      , Func<A, B> map
      , Func<B, B, B> sum
      )
    =>
        xs.Aggregate(zero, (b, a) => sum(b, map(a)));

    public static B FoldLeft<A, B>
      ( IEnumerable<A> e
      , B z
      , Func<B, A, B> f
      )
    =>
        e.Aggregate(z, f);

    public static Step<B> FoldRight<A, B>
      ( this IEnumerable<A> e
      , B x
      , Func<A, Step<B>, Step<B>> f
      )
    =>
        FoldRight(e.GetEnumerator(), x, f);

    private static IEnumerable<A> Iterate<A>(Func<A, A> next, A a)
    {
        while (true)
        {
            yield return a;
            a = next(a);
        }
    }

    private static Step<B> FoldRight<A, B>
      ( IEnumerator<A> e
      , B x
      , Func<A, Step<B>, Step<B>> f
      )
    =>
        Suspend
          ( () =>
                e.MoveNext()
                ? f(e.Current, FoldRight(e, x, f))
                : Done(x)
          );
}

}
