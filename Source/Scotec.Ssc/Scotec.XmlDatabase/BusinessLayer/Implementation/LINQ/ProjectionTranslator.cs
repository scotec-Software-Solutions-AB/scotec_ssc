#region

using System;
using System.Linq.Expressions;

#endregion


namespace Scotec.XMLDatabase.LINQ
{
    internal partial class QueryTranslator
    {
        #region Nested type: ProjectionTranslator

        private class ProjectionTranslator : ExpressionVisitor
        {
            private LambdaExpression _projectionLambda;


            public ProjectionTranslator( IBusinessObject context )
            {
                Context = context;
            }


            public IBusinessObject Context { get; private set; }

            internal string ProjectionClause
            {
                //get { return FormatHelper.FormatColumnNamesInSequence(_memebers); }
                get { return string.Empty; }
            }

            //internal ReadOnlyCollection<DatabaseMetaDataMember> DataMembers
            //{
            //  get { return _memebers.AsReadOnly(); }
            //}

            internal LambdaExpression ProjectionLambda
            {
                get { return _projectionLambda; }
            }


            internal void Translate( LambdaExpression lambda )
            {
                _projectionLambda = lambda;
                base.VisitLambda( lambda );
            }


            protected override Expression VisitMemberAccess( MemberExpression m )
            {
                // get the selected columns

                if( m.Expression == null || m.Expression.NodeType != ExpressionType.Parameter )
                    throw new NotSupportedException( string.Format( "The member '{0}' is not supported", m.Member.Name ) );

                // use the mapping metadata and find the name of this member in the database
                var declaringType = m.Member.DeclaringType;
                //DatabaseMetaTable metaTable = Context.GetTable(declaringType);

                //if (metaTable == null)
                //  throw new Exception(string.Format("It was not possible to get metadat for {0}!", declaringType.Name));

                //DatabaseMetaDataMember metaMember = metaTable.GetPersistentDataMember(m.Member);

                //if (metaMember == null)
                //  throw new Exception(string.Format("The member {0} in where expression cannot be found on the {1} class", m.Member.Name, declaringType.Name));

                //_memebers.Add(metaMember);

                return m;
            }
        }

        #endregion
    }
}
