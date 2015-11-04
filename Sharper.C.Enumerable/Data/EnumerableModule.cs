﻿using System;
using System.Collections.Generic;
using System.Linq;
using Sharper.C.Control;

namespace Sharper.C.Enumerable.Data
{

using static TrampolineModule;

public static class EnumerableModule
{
    public static B FoldLeft<A, B>(this IEnumerable<A> e, B x, Func<B, A, B> f)
    =>
        e.Aggregate(x, f);

    public static B FoldRight<A, B>(this IEnumerable<A> e, B x, Func<A, B, B> f)
    =>
        e.Reverse().Aggregate(x, (b, a) => f(a, b));

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
