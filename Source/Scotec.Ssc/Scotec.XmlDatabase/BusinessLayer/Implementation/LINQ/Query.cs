#region

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

#endregion


namespace Scotec.XMLDatabase.LINQ
{
    public class Query
    {
        public Query( IBusinessObject context )
        {
            Context = context;
        }


        public IBusinessObject Context { get; private set; }
    }

    public class Query<TBO> : Query, IQueryable<TBO>, IQueryProvider where TBO : IBusinessObject
    {
        public Query( IBusinessObject context )
                : base( context )
        {
        }


        #region IQueryable<TBO> Members

        public IEnumerator<TBO> GetEnumerator()
        {
            return ((IEnumerable<TBO>)Provider.Execute( Expression.Constant( this ) )).GetEnumerator();
        }


        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }


        public Expression Expression
        {
            get { return Expression.Constant( this ); }
        }

        public Type ElementType
        {
            get { return typeof( TBO ); }
        }

        public IQueryProvider Provider
        {
            get { return this; }
        }

        #endregion


        #region IQueryProvider Members

        public IQueryable CreateQuery( Expression expression )
        {
            return this;
        }


        public IQueryable<TElement> CreateQuery<TElement>( Expression expression )
        {
            return (IQueryable<TElement>)Execute( expression );
        }


        public object Execute( Expression expression )
        {
            var translator = new QueryTranslator( Context );

            // Build XPath query
            var info = translator.Translate(expression);

            // execute the XPath query
            return ((IBusinessObjectQuery)Context).Execute( info.QueryText );
        }


        public TResult Execute<TResult>( Expression expression )
        {
            return (TResult)Execute( expression );
        }

        #endregion
    }
}
