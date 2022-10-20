﻿#region

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;

#endregion


namespace Scotec.XMLDatabase.LINQ
{
    internal abstract class ExpressionVisitor
    {
        protected virtual Expression Visit( Expression exp )
        {
            if( exp == null )
                return exp;
            switch( exp.NodeType )
            {
                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                case ExpressionType.ArrayIndex:
                case ExpressionType.Coalesce:
                case ExpressionType.Divide:
                case ExpressionType.Equal:
                case ExpressionType.ExclusiveOr:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.LeftShift:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.Modulo:
                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                case ExpressionType.NotEqual:
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                case ExpressionType.Power:
                case ExpressionType.RightShift:
                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                    return VisitBinary( (BinaryExpression)exp );

                case ExpressionType.ArrayLength:
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                case ExpressionType.Negate:
                case ExpressionType.NegateChecked:
                case ExpressionType.Not:
                case ExpressionType.Quote:
                case ExpressionType.TypeAs:
                    return VisitUnary( (UnaryExpression)exp );

                case ExpressionType.Call:
                    return VisitMethodCall( (MethodCallExpression)exp );

                case ExpressionType.Conditional:
                    return VisitConditional( (ConditionalExpression)exp );

                case ExpressionType.Constant:
                    return VisitConstant( (ConstantExpression)exp );

                case ExpressionType.Invoke:
                    return VisitInvocation( (InvocationExpression)exp );

                case ExpressionType.Lambda:
                    return VisitLambda( (LambdaExpression)exp );

                case ExpressionType.ListInit:
                    return VisitListInit( (ListInitExpression)exp );

                case ExpressionType.MemberAccess:
                    return VisitMemberAccess( (MemberExpression)exp );

                case ExpressionType.MemberInit:
                    return VisitMemberInit( (MemberInitExpression)exp );

                case ExpressionType.New:
                    return VisitNew( (NewExpression)exp );

                case ExpressionType.NewArrayInit:
                case ExpressionType.NewArrayBounds:
                    return VisitNewArray( (NewArrayExpression)exp );

                case ExpressionType.Parameter:
                    return VisitParameter( (ParameterExpression)exp );

                case ExpressionType.TypeIs:
                    return VisitTypeIs( (TypeBinaryExpression)exp );
                default:
                    throw new Exception( string.Format( "Unhandled expression type: '{0}'", exp.NodeType ) );
            }
        }


        protected virtual Expression VisitBinary( BinaryExpression b )
        {
            var left = Visit( b.Left );
            var right = Visit( b.Right );
            if( (left == b.Left) && (right == b.Right) )
                return b;
            return Expression.MakeBinary( b.NodeType, left, right, b.IsLiftedToNull, b.Method );
        }


        protected virtual MemberBinding VisitBinding( MemberBinding binding )
        {
            switch( binding.BindingType )
            {
                case MemberBindingType.Assignment:
                    return VisitMemberAssignment( (MemberAssignment)binding );

                case MemberBindingType.MemberBinding:
                    return VisitMemberMemberBinding( (MemberMemberBinding)binding );

                case MemberBindingType.ListBinding:
                    return VisitMemberListBinding( (MemberListBinding)binding );
                default:
                    throw new Exception( string.Format( "Unhandled binding type '{0}'", binding.BindingType ) );
            }
        }


        protected virtual IEnumerable<MemberBinding> VisitBindingList( ReadOnlyCollection<MemberBinding> original )
        {
            List<MemberBinding> list = null;
            var num = 0x0;
            var count = original.Count;
            while( num < count )
            {
                var item = VisitBinding( original[num] );
                if( list != null )
                    list.Add( item );
                else if( item != original[num] )
                {
                    list = new List<MemberBinding>( count );
                    for( var i = 0; i < num; i++ )
                        list.Add( original[i] );
                    list.Add( item );
                }
                num++;
            }
            if( list != null )
                return list;
            return original;
        }


        protected virtual Expression VisitConditional( ConditionalExpression c )
        {
            var test = Visit( c.Test );
            var ifTrue = Visit( c.IfTrue );
            var ifFalse = Visit( c.IfFalse );
            if( ((test == c.Test) && (ifTrue == c.IfTrue)) && (ifFalse == c.IfFalse) )
                return c;
            return Expression.Condition( test, ifTrue, ifFalse );
        }


        protected virtual Expression VisitConstant( ConstantExpression c )
        {
            return c;
        }


        protected virtual ElementInit VisitElementInitializer( ElementInit initializer )
        {
            var arguments = VisitExpressionList( initializer.Arguments );
            if( arguments != initializer.Arguments )
                return Expression.ElementInit( initializer.AddMethod, arguments );
            return initializer;
        }


        protected virtual IEnumerable<ElementInit> VisitElementInitializerList( ReadOnlyCollection<ElementInit> original )
        {
            List<ElementInit> list = null;
            var num = 0;
            var count = original.Count;
            while( num < count )
            {
                var item = VisitElementInitializer( original[num] );
                if( list != null )
                    list.Add( item );
                else if( item != original[num] )
                {
                    list = new List<ElementInit>( count );
                    for( var i = 0; i < num; i++ )
                        list.Add( original[i] );
                    list.Add( item );
                }
                num++;
            }
            if( list != null )
                return list;
            return original;
        }


        protected virtual ReadOnlyCollection<Expression> VisitExpressionList( ReadOnlyCollection<Expression> original )
        {
            List<Expression> list = null;
            var num = 0x0;
            var count = original.Count;
            while( num < count )
            {
                var item = Visit( original[num] );
                if( list != null )
                    list.Add( item );
                else if( item != original[num] )
                {
                    list = new List<Expression>( count );
                    for( var i = 0; i < num; i++ )
                        list.Add( original[i] );
                    list.Add( item );
                }
                num++;
            }
            if( list != null )
                return new ReadOnlyCollection<Expression>( list );
            return original;
        }


        protected virtual Expression VisitInvocation( InvocationExpression iv )
        {
            IEnumerable<Expression> arguments = VisitExpressionList( iv.Arguments );
            var expression = Visit( iv.Expression );
            if( (arguments == iv.Arguments) && (expression == iv.Expression) )
                return iv;
            return Expression.Invoke( expression, arguments );
        }


        protected virtual Expression VisitLambda( LambdaExpression lambda )
        {
            var body = Visit( lambda.Body );
            if( body != lambda.Body )
                return Expression.Lambda( lambda.Type, body, lambda.Parameters );
            return lambda;
        }


        protected virtual Expression VisitListInit( ListInitExpression init )
        {
            var newExpression = VisitNew( init.NewExpression );
            var initializers = VisitElementInitializerList( init.Initializers );
            if( (newExpression == init.NewExpression) && (initializers == init.Initializers) )
                return init;
            return Expression.ListInit( newExpression, initializers );
        }


        protected virtual Expression VisitMemberAccess( MemberExpression m )
        {
            var expression = Visit( m.Expression );
            if( expression != m.Expression )
                return Expression.MakeMemberAccess( expression, m.Member );
            return m;
        }


        protected virtual MemberAssignment VisitMemberAssignment( MemberAssignment assignment )
        {
            var expression = Visit( assignment.Expression );
            if( expression != assignment.Expression )
                return Expression.Bind( assignment.Member, expression );
            return assignment;
        }


        protected virtual Expression VisitMemberInit( MemberInitExpression init )
        {
            var newExpression = VisitNew( init.NewExpression );
            var bindings = VisitBindingList( init.Bindings );
            if( (newExpression == init.NewExpression) && (bindings == init.Bindings) )
                return init;
            return Expression.MemberInit( newExpression, bindings );
        }


        protected virtual MemberListBinding VisitMemberListBinding( MemberListBinding binding )
        {
            var initializers = VisitElementInitializerList( binding.Initializers );
            if( initializers != binding.Initializers )
                return Expression.ListBind( binding.Member, initializers );
            return binding;
        }


        protected virtual MemberMemberBinding VisitMemberMemberBinding( MemberMemberBinding binding )
        {
            var bindings = VisitBindingList( binding.Bindings );
            if( bindings != binding.Bindings )
                return Expression.MemberBind( binding.Member, bindings );
            return binding;
        }


        protected virtual Expression VisitMethodCall( MethodCallExpression m )
        {
            var instance = Visit( m.Object );
            IEnumerable<Expression> arguments = VisitExpressionList( m.Arguments );
            if( (instance == m.Object) && (arguments == m.Arguments) )
                return m;
            return Expression.Call( instance, m.Method, arguments );
        }


        protected virtual NewExpression VisitNew( NewExpression nex )
        {
            IEnumerable<Expression> arguments = VisitExpressionList( nex.Arguments );
            if( arguments == nex.Arguments )
                return nex;
            if( nex.Members != null )
                return Expression.New( nex.Constructor, arguments, nex.Members );
            return Expression.New( nex.Constructor, arguments );
        }


        protected virtual Expression VisitNewArray( NewArrayExpression na )
        {
            IEnumerable<Expression> initializers = VisitExpressionList( na.Expressions );
            if( initializers == na.Expressions )
                return na;
            if( na.NodeType == ExpressionType.NewArrayInit )
                return Expression.NewArrayInit( na.Type.GetElementType(), initializers );
            return Expression.NewArrayBounds( na.Type.GetElementType(), initializers );
        }


        protected virtual Expression VisitParameter( ParameterExpression p )
        {
            return p;
        }


        protected virtual Expression VisitTypeIs( TypeBinaryExpression b )
        {
            var expression = Visit( b.Expression );
            if( expression != b.Expression )
                return Expression.TypeIs( expression, b.TypeOperand );
            return b;
        }


        protected virtual Expression VisitUnary( UnaryExpression u )
        {
            var operand = Visit( u.Operand );
            if( operand != u.Operand )
                return Expression.MakeUnary( u.NodeType, operand, u.Type, u.Method );
            return u;
        }
    }
}
