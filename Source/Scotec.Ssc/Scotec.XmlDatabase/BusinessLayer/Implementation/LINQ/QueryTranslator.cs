#region

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

#endregion


namespace Scotec.XMLDatabase.LINQ
{
    internal partial class QueryTranslator : ExpressionVisitor
    {
        private readonly IBusinessSession _session;
        //readonly OrderByTranslator _orderByTranslator;
        //WhereTranslator _whereTranslator;
        private ProjectionTranslator _selectTranslator;
        //TakeTranslator _takeTranslator;


        internal QueryTranslator( IBusinessObject context )
        {
            Context = context;
            //_orderByTranslator = new OrderByTranslator(session);
        }


        public IBusinessObject Context { get; private set; }


        internal QueryInfo Translate( Expression query )
        {
            Visit( query );
            return ConvertToExecutableQuery( query );
        }


        private QueryInfo ConvertToExecutableQuery( Expression query )
        {
            // find the source
            //DatabaseMetaTable source;
            //if (!GetSourceTable(query, out source))
            //    throw new NotSupportedException("This query expression is not supported!");
            IBusinessObject source;
            GetBusinessObject( query, out source );


            var useDefault = false;
            var sb = new StringBuilder();

            // SELECT
            sb.Append( "./*/ " );

            // TOP
            //if (_takeTranslator != null && _takeTranslator.Count.HasValue)
            //{
            //    useDefault = _takeTranslator.UseDefault;

            //    sb.Append("TOP ");
            //    sb.Append(_takeTranslator.Count);
            //    sb.Append(" ");
            //}

            // PROJECTION
            //if (_selectTranslator == null || !_selectTranslator.DataMembers.Any())
            if( _selectTranslator == null )
            {
                // project on all the mapped columns
                //_selectTranslator = new ProjectionTranslator(_model, source.PersistentDataMembers);
                _selectTranslator = new ProjectionTranslator( Context );
            }
            //if (!_selectTranslator.DataMembers.Any())
            //    throw new Exception("There are no items for projection in this query!");

            sb.Append(_selectTranslator.ProjectionClause);

            // FROM
            sb.Append( " FROM " );
            //sb.AppendLine(source.TableName);

            // WHERE
            //if (_whereTranslator != null)
            //{
            //    string where = _whereTranslator.WhereClause;
            //    if (!string.IsNullOrEmpty(where))
            //    {
            //        sb.Append("WHERE ");
            //        sb.AppendLine(where);
            //    }
            //}

            // ORDER BY
            //if (_orderByTranslator != null)
            //{
            //    string orderby = _orderByTranslator.OrderByClause;
            //    if (!string.IsNullOrEmpty(orderby))
            //    {
            //        sb.Append("ORDER BY ");
            //        sb.AppendLine(orderby);
            //    }
            //}

            return new QueryInfo( GetResultShape( query ), sb.ToString() )
                   {
                           //SourceMetadata = source,
                           LambdaExpression = _selectTranslator.ProjectionLambda,
                           UseDefault = useDefault
                   };
        }


        /// <summary>
        ///   What will this query return?
        ///   - a single entity?
        ///   - a sequence of entities?
        ///   - or nothing?
        /// </summary>
        /// <param name = "query"></param>
        /// <returns></returns>
        private ResultShape GetResultShape( Expression query )
        {
            var lambda = query as LambdaExpression;
            if( lambda != null )
                query = lambda.Body;

            if( query.Type == typeof( void ) )
                return ResultShape.None;

            //if (query.Type == typeof(IMultipleResults))
            //    throw new NotSupportedException("Multiple result shape is not supported");

            var methodExp = query as MethodCallExpression;
            if( methodExp != null && ((methodExp.Method.DeclaringType == typeof( Queryable )) || (methodExp.Method.DeclaringType == typeof( Enumerable ))) )
            {
                var str = methodExp.Method.Name;
                if( str != null && (str == "First" || str == "FirstOrDefault" || str == "Single" || str == "SingleOrDefault") )
                    return ResultShape.Singleton;
            }
            return ResultShape.Sequence;
        }


        private static bool GetBusinessObject( Expression query, out IBusinessObject source )
        {
            source = null;

            var me = query as MethodCallExpression;
            ConstantExpression ce;
            if( me == null )
                ce = query as ConstantExpression;
            else
            {
                // recurse down all the method calls to find the source object
                while( true )
                {
                    if( me.Arguments[0] is MethodCallExpression )
                        me = me.Arguments[0] as MethodCallExpression;
                    else
                        break;
                }

                ce = me.Arguments[0] as ConstantExpression;
            }

            if( ce != null )
            {
                var obj = ce.Value as Query;
                if( obj != null )
                {
                    source = obj.Context;
                    return true;
                }
            }

            return false;
        }


        #region Visitors

        protected override Expression VisitMethodCall( MethodCallExpression mc )
        {
            var declaringType = mc.Method.DeclaringType;
            if( declaringType != typeof( Queryable ) )
            {
                throw new NotSupportedException(
                        "Invalid Sequence Operator Call. The type for the operator is not Queryable!" );
            }

            switch( mc.Method.Name )
            {
                    //case "Where":
                    //    // is this really a proper Where?
                    //    var whereLambda = GetLambdaWithParamCheck(mc);
                    //    if (whereLambda == null)
                    //        break;

                    //    VisitWhere(whereLambda);
                    //    break;
                    //case "OrderBy":
                    //case "ThenBy":
                    //    // is this really a proper Order By?
                    //    var orderLambda = GetLambdaWithParamCheck(mc);
                    //    if (orderLambda == null)
                    //        break;

                    //    VisitOrderBy(orderLambda, OrderDirection.Ascending);
                    //    break;
                    //case "OrderByDescending":
                    //case "ThenByDescending":
                    //    // is this really a proper Order By Descending?
                    //    var orderDescLambda = GetLambdaWithParamCheck(mc);
                    //    if (orderDescLambda == null)
                    //        break;

                    //    VisitOrderBy(orderDescLambda, OrderDirection.Descending);
                    //    break;
                case "Select":
                    // is this really a proper Select?
                    var selectLambda = GetLambdaWithParamCheck( mc );
                    if( selectLambda == null )
                        break;

                    VisitSelect( selectLambda );
                    break;

                    //case "Take":
                    //    if (mc.Arguments.Count != 2)
                    //        break;

                    //    VisitTake(mc.Arguments[1]);
                    //    break;

                    //case "First":
                    //    // This custom provider does not support the use of a First operator
                    //    // that takes a predicate. Therefore we check to ensure that no more
                    //    // than one argument is provided.
                    //    if (mc.Arguments.Count != 1)
                    //        break;

                    //    VisitFirst(false);
                    //    break;

                    //case "FirstOrDefault":
                    //    // This custom provider does not support the use of a FirstOrDefault
                    //    // operator that takes a predicate. Therefore we check to ensure that
                    //    // no more than one argument is provided.
                    //    if (mc.Arguments.Count != 1)
                    //        break;

                    //    VisitFirst(true);
                    //    break;

                default:
                    return base.VisitMethodCall( mc );
            }

            Visit( mc.Arguments[0] );
            return mc;
        }


        //private void VisitWhere(LambdaExpression predicate)
        //{
        //    // this custom provider cannot support more 
        //    // than one Where query operator in a LINQ query
        //    if (_whereTranslator != null)
        //        throw new NotSupportedException(
        //          "You cannot have more than one Where in the expression");
        //    _whereTranslator = new WhereTranslator(_session);
        //    _whereTranslator.Translate(predicate);
        //}
        /// <param name = "predicate">The lambda expression parameter to the Where extension method</param>
        /// <param name = "predicate">The lambda expression parameter to the Select extension method</param>
        private void VisitSelect( LambdaExpression predicate )
        {
            if( _selectTranslator != null )
                throw new NotSupportedException( "You cannot have more than 1 Select in the expression" );

            _selectTranslator = new ProjectionTranslator( Context );
            _selectTranslator.Translate( predicate );
        }


        //private void VisitOrderBy(LambdaExpression predicate, OrderDirection direction)
        //{
        //    _orderByTranslator.Visit(predicate, direction);
        //}

        //private void VisitTake(Expression takeValue)
        //{
        //    if (_takeTranslator != null)
        //        throw new NotSupportedException("You cannot have more than 1 Take/First/FirstOrDefault in the expression");

        //    _takeTranslator = new TakeTranslator();
        //    _takeTranslator.Translate(takeValue);
        //}


        //private void VisitFirst(bool useDefault)
        //{
        //    if (_takeTranslator != null)
        //        throw new NotSupportedException("You cannot have more than 1 Take/First/FirstOrDefault in the expression");

        //    _takeTranslator = TakeTranslator.GetNewFirstTranslator(useDefault);
        //}

        #endregion


        #region Helper methods

        /// <summary>
        ///   Check to see if the expression is valid for
        ///   Select, Where, OrderBy, ThenBy, OrderByDescending and ThenByDescending
        ///   and then return the lmbda section
        /// </summary>
        /// <returns></returns>
        private static LambdaExpression GetLambdaWithParamCheck( MethodCallExpression mc )
        {
            if( mc.Arguments.Count != 2 || !IsLambda( mc.Arguments[1] ) )
                return null;

            var lambda = GetLambda( mc.Arguments[1] );
            return (lambda.Parameters.Count != 1) ? null : lambda;
        }


        private static bool IsLambda( Expression expression )
        {
            return RemoveQuotes( expression ).NodeType == ExpressionType.Lambda;
        }


        private static LambdaExpression GetLambda( Expression expression )
        {
            return RemoveQuotes( expression ) as LambdaExpression;
        }


        private static Expression RemoveQuotes( Expression expression )
        {
            while( expression.NodeType == ExpressionType.Quote )
                expression = ((UnaryExpression)expression).Operand;
            return expression;
        }

        #endregion
    }
}
