// ------------------------------------------------------------------------------
// -- Copyright ERTMS Solutions
// -- Licensed under the EUPL V.1.1
// -- http://joinup.ec.europa.eu/software/page/eupl/licence-eupl
// --
// -- This file is part of ERTMSFormalSpec software and documentation
// --
// --  ERTMSFormalSpec is free software: you can redistribute it and/or modify
// --  it under the terms of the EUPL General Public License, v.1.1
// --
// -- ERTMSFormalSpec is distributed in the hope that it will be useful,
// -- but WITHOUT ANY WARRANTY; without even the implied warranty of
// -- MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// --
// ------------------------------------------------------------------------------
namespace DataDictionary.Interpreter
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Hand written visitor for expressions
    /// </summary>
    public class Visitor
    {
        /// <summary>
        /// Visits a tree node
        /// </summary>
        /// <param name="interpreterTreeNode"></param>
        protected virtual void visitInterpreterTreeNode(InterpreterTreeNode interpreterTreeNode)
        {
            Designator designator = interpreterTreeNode as Designator;
            if (designator != null)
            {
                VisitDesignator(designator);
            }

            Term term = interpreterTreeNode as Term;
            if (term != null)
            {
                VisitTerm(term);
            }

            Expression expression = interpreterTreeNode as Expression;
            if (expression != null)
            {
                VisitExpression(expression);
            }

            Statement.Statement statement = interpreterTreeNode as Statement.Statement;
            if (statement != null)
            {
                VisitStatement(statement);
            }
        }

        /// <summary>
        /// Visits a designator
        /// </summary>
        /// <param name="?"></param>
        protected virtual void VisitDesignator(Designator designator)
        {
        }

        /// <summary>
        /// Visits a term
        /// </summary>
        /// <param name="?"></param>
        protected virtual void VisitTerm(Term term)
        {
            if (term != null)
            {
                if (term.Designator != null)
                {
                    VisitDesignator(term.Designator);
                }

                if (term.LiteralValue != null)
                {
                    VisitExpression(term.LiteralValue);
                }
            }
        }

        /// <summary>
        /// Visits an expression
        /// </summary>
        /// <param name="?"></param>
        protected virtual void VisitExpression(Expression expression)
        {
            if (expression != null)
            {
                if (expression is BinaryExpression)
                {
                    VisitBinaryExpression((BinaryExpression)expression);
                }
                else if (expression is Call)
                {
                    VisitCall((Call)expression);
                }
                else if (expression is DerefExpression)
                {
                    VisitDerefExpression((DerefExpression)expression);
                }
                else if (expression is FunctionExpression)
                {
                    VisitFunctionExpression((FunctionExpression)expression);
                }
                else if (expression is ListExpression)
                {
                    VisitListExpression((ListExpression)expression);
                }
                else if (expression is NumberExpression)
                {
                    VisitNumberExpression((NumberExpression)expression);
                }
                else if (expression is StringExpression)
                {
                    VisitStringExpression((StringExpression)expression);
                }
                else if (expression is StructExpression)
                {
                    VisitStructExpression((StructExpression)expression);
                }
                else if (expression is UnaryExpression)
                {
                    VisitUnaryExpression((UnaryExpression)expression);
                }
                else if (expression is ListOperators.CountExpression)
                {
                    VisitCountExpression((ListOperators.CountExpression)expression);
                }
                else if (expression is ListOperators.FirstExpression)
                {
                    VisitFirstExpression((ListOperators.FirstExpression)expression);
                }
                else if (expression is ListOperators.ForAllExpression)
                {
                    VisitForAllExpression((ListOperators.ForAllExpression)expression);
                }
                else if (expression is ListOperators.LastExpression)
                {
                    VisitLastExpression((ListOperators.LastExpression)expression);
                }
                else if (expression is ListOperators.MapExpression)
                {
                    VisitMapExpression((ListOperators.MapExpression)expression);
                }
                else if (expression is ListOperators.ReduceExpression)
                {
                    VisitReduceExpression((ListOperators.ReduceExpression)expression);
                }
                else if (expression is ListOperators.SumExpression)
                {
                    VisitSumExpression((ListOperators.SumExpression)expression);
                }
                else if (expression is ListOperators.ThereIsExpression)
                {
                    VisitThereIsExpression((ListOperators.ThereIsExpression)expression);
                }
            }
        }

        /// <summary>
        /// Visits a condition based list expression
        /// </summary>
        /// <param name="conditionBasedListExpression"></param>
        protected virtual void VisitConditionBasedListExpression(ListOperators.ConditionBasedListExpression conditionBasedListExpression)
        {
            if (conditionBasedListExpression != null)
            {
                if (conditionBasedListExpression.ListExpression != null)
                {
                    VisitExpression(conditionBasedListExpression.ListExpression);
                }
                if (conditionBasedListExpression.Condition != null)
                {
                    VisitExpression(conditionBasedListExpression.Condition);
                }
            }
        }
        /// <summary>
        /// Visits a THERE IS expression
        /// </summary>
        /// <param name="thereIsExpression"></param>
        protected virtual void VisitThereIsExpression(ListOperators.ThereIsExpression thereIsExpression)
        {
            if (thereIsExpression != null)
            {
                VisitConditionBasedListExpression(thereIsExpression);
            }
        }

        /// <summary>
        /// Visits a SUM expression
        /// </summary>
        /// <param name="sumExpression"></param>
        protected virtual void VisitSumExpression(ListOperators.SumExpression sumExpression)
        {
            if (sumExpression != null)
            {
                VisitConditionBasedListExpression(sumExpression);
                if (sumExpression != null)
                {
                    VisitExpression(sumExpression.IteratorExpression);
                }
            }
        }

        /// <summary>
        /// Visits an expression based list expression
        /// </summary>
        /// <param name="expressionBasedListExpression"></param>
        protected virtual void VisitExpressionBasedListExpression(ListOperators.ExpressionBasedListExpression expressionBasedListExpression)
        {
            if (expressionBasedListExpression != null)
            {
                VisitConditionBasedListExpression(expressionBasedListExpression);
                if (expressionBasedListExpression.IteratorExpression != null)
                {
                    VisitExpression(expressionBasedListExpression.IteratorExpression);
                }
            }
        }

        /// <summary>
        /// Visits a REDUCE expression
        /// </summary>
        /// <param name="reduceExpression"></param>
        protected virtual void VisitReduceExpression(ListOperators.ReduceExpression reduceExpression)
        {
            if (reduceExpression != null)
            {
                VisitExpressionBasedListExpression(reduceExpression);
                if (reduceExpression.InitialValue != null)
                {
                    VisitExpression(reduceExpression.InitialValue);
                }
            }
        }

        /// <summary>
        /// Visits a MAP expression
        /// </summary>
        /// <param name="mapExpression"></param>
        protected virtual void VisitMapExpression(ListOperators.MapExpression mapExpression)
        {
            if (mapExpression != null)
            {
                VisitExpressionBasedListExpression(mapExpression);
            }
        }

        /// <summary>
        /// Visits a LAST expression
        /// </summary>
        /// <param name="lastExpression"></param>
        protected virtual void VisitLastExpression(ListOperators.LastExpression lastExpression)
        {
            if (lastExpression != null)
            {
                VisitConditionBasedListExpression(lastExpression);
            }
        }

        /// <summary>
        /// Visits a FOR ALL expression
        /// </summary>
        /// <param name="forAllExpression"></param>
        protected virtual void VisitForAllExpression(ListOperators.ForAllExpression forAllExpression)
        {
            if (forAllExpression != null)
            {
                VisitConditionBasedListExpression(forAllExpression);
            }
        }

        /// <summary>
        /// Visits a FIRST expression
        /// </summary>
        /// <param name="firstExpression"></param>
        protected virtual void VisitFirstExpression(ListOperators.FirstExpression firstExpression)
        {
            if (firstExpression != null)
            {
                VisitConditionBasedListExpression(firstExpression);
            }
        }

        /// <summary>
        /// Visits a COUNT expression
        /// </summary>
        /// <param name="countExpression"></param>
        protected virtual void VisitCountExpression(ListOperators.CountExpression countExpression)
        {
            if (countExpression != null)
            {
                VisitConditionBasedListExpression(countExpression);
            }
        }

        /// <summary>
        /// Visits a unary expression
        /// </summary>
        /// <param name="unaryExpression"></param>
        protected virtual void VisitUnaryExpression(UnaryExpression unaryExpression)
        {
            if (unaryExpression.Expression != null)
            {
                VisitExpression(unaryExpression.Expression);
            }
            if (unaryExpression.Term != null)
            {
                VisitTerm(unaryExpression.Term);
            }
        }

        /// <summary>
        /// Visits a struct expression
        /// </summary>
        /// <param name="structExpression"></param>
        protected virtual void VisitStructExpression(StructExpression structExpression)
        {
            if (structExpression.Structure != null)
            {
                VisitExpression(structExpression.Structure);
            }
            foreach (KeyValuePair<Designator, Expression> pair in structExpression.Associations)
            {
                if (pair.Key != null)
                {
                    VisitDesignator(pair.Key);
                }
                if (pair.Value != null)
                {
                    VisitExpression(pair.Value);
                }
            }
        }

        /// <summary>
        /// Visits a String expression
        /// </summary>
        /// <param name="stringExpression"></param>
        protected virtual void VisitStringExpression(StringExpression stringExpression)
        {
        }

        /// <summary>
        /// Visits a Number expression
        /// </summary>
        /// <param name="numberExpression"></param>
        protected virtual void VisitNumberExpression(NumberExpression numberExpression)
        {
        }

        /// <summary>
        /// Visits a List expression
        /// </summary>
        /// <param name="listExpression"></param>
        protected virtual void VisitListExpression(ListExpression listExpression)
        {
            foreach (Expression expression in listExpression.ListElements)
            {
                if (expression != null)
                {
                    VisitExpression(expression);
                }
            }
        }

        /// <summary>
        /// Visits a Function expression
        /// </summary>
        /// <param name="functionExpression"></param>
        protected virtual void VisitFunctionExpression(FunctionExpression functionExpression)
        {
            if (functionExpression.Expression != null)
            {
                VisitExpression(functionExpression.Expression);
            }
        }

        /// <summary>
        /// Visits a Deref expression
        /// </summary>
        /// <param name="derefExpression"></param>
        protected virtual void VisitDerefExpression(DerefExpression derefExpression)
        {
            foreach (Expression expression in derefExpression.Arguments)
            {
                if (expression != null)
                {
                    VisitExpression(expression);
                }
            }
        }

        /// <summary>
        /// Visits a Call expression
        /// </summary>
        /// <param name="call"></param>
        protected virtual void VisitCall(Call call)
        {
            if (call.Called != null)
            {
                VisitExpression(call.Called);
            }
            foreach (Expression expression in call.ActualParameters)
            {
                if (expression != null)
                {
                    VisitExpression(expression);
                }
            }
            foreach (KeyValuePair<Designator, Expression> pair in call.NamedActualParameters)
            {
                if (pair.Key != null)
                {
                    VisitDesignator(pair.Key);
                }
                if (pair.Value != null)
                {
                    VisitExpression(pair.Value);
                }
            }
        }

        /// <summary>
        /// Visits a Binary expression
        /// </summary>
        /// <param name="binaryExpression"></param>
        protected virtual void VisitBinaryExpression(BinaryExpression binaryExpression)
        {
            if (binaryExpression.Left != null)
            {
                VisitExpression(binaryExpression.Left);
            }
            if (binaryExpression.Right != null)
            {
                VisitExpression(binaryExpression.Right);
            }
        }

        /// <summary>
        /// Visits a statement
        /// </summary>
        /// <param name="?"></param>
        protected virtual void VisitStatement(Statement.Statement statement)
        {
            if (statement != null)
            {
                if (statement is Statement.ApplyStatement)
                {
                    VisitApplyStatement((Statement.ApplyStatement)statement);
                }
                else if (statement is Statement.InsertStatement)
                {
                    VisitInsertStatement((Statement.InsertStatement)statement);
                }
                else if (statement is Statement.ProcedureCallStatement)
                {
                    VisitProcedureCallStatement((Statement.ProcedureCallStatement)statement);
                }
                else if (statement is Statement.RemoveStatement)
                {
                    VisitRemoveStatement((Statement.RemoveStatement)statement);
                }
                else if (statement is Statement.ReplaceStatement)
                {
                    VisitReplaceStatement((Statement.ReplaceStatement)statement);
                }
                else if (statement is Statement.VariableUpdateStatement)
                {
                    VisitVariableUpdateStatement((Statement.VariableUpdateStatement)statement);
                }
            }
        }

        /// <summary>
        /// Visits a Variable update statement
        /// </summary>
        /// <param name="variableUpdateStatement"></param>
        protected virtual void VisitVariableUpdateStatement(Statement.VariableUpdateStatement variableUpdateStatement)
        {
            if (variableUpdateStatement.VariableIdentification != null)
            {
                VisitExpression(variableUpdateStatement.VariableIdentification);
            }
            if (variableUpdateStatement.Expression != null)
            {
                VisitExpression(variableUpdateStatement.Expression);
            }
        }

        /// <summary>
        /// Visits a REPLACE statement
        /// </summary>
        /// <param name="replaceStatement"></param>
        protected virtual void VisitReplaceStatement(Statement.ReplaceStatement replaceStatement)
        {
            if (replaceStatement.ListExpression != null)
            {
                VisitExpression(replaceStatement.ListExpression);
            }
            if (replaceStatement.Condition != null)
            {
                VisitExpression(replaceStatement.Condition);
            }
            if (replaceStatement.Value != null)
            {
                VisitExpression(replaceStatement.Value);
            }
        }

        /// <summary>
        /// Visits a REMOVE statement
        /// </summary>
        /// <param name="removeStatement"></param>
        protected virtual void VisitRemoveStatement(Statement.RemoveStatement removeStatement)
        {
            if (removeStatement.ListExpression != null)
            {
                VisitExpression(removeStatement.ListExpression);
            }
            if (removeStatement.Condition != null)
            {
                VisitExpression(removeStatement.Condition);
            }
        }

        /// <summary>
        /// Visits a Procedure call statement
        /// </summary>
        /// <param name="procedureCallStatement"></param>
        protected virtual void VisitProcedureCallStatement(Statement.ProcedureCallStatement procedureCallStatement)
        {
            if (procedureCallStatement.Call != null)
            {
                VisitExpression(procedureCallStatement.Call);
            }
        }

        /// <summary>
        /// Visits an INSERT statement
        /// </summary>
        /// <param name="insertStatement"></param>
        protected virtual void VisitInsertStatement(Statement.InsertStatement insertStatement)
        {
            if (insertStatement.ListExpression != null)
            {
                VisitExpression(insertStatement.ListExpression);
            }
            if (insertStatement.Value != null)
            {
                VisitExpression(insertStatement.Value);
            }
            if (insertStatement.ReplaceElement != null)
            {
                VisitExpression(insertStatement.ReplaceElement);
            }
        }

        /// <summary>
        /// Visits an APPLY statement
        /// </summary>
        /// <param name="applyStatement"></param>
        protected virtual void VisitApplyStatement(Statement.ApplyStatement applyStatement)
        {
            if (applyStatement.Call != null)
            {
                VisitProcedureCallStatement(applyStatement.Call);
            }
            if (applyStatement.ConditionExpression != null)
            {
                VisitExpression(applyStatement.ConditionExpression);
            }
            if (applyStatement.ListExpression != null)
            {
                VisitExpression(applyStatement.ListExpression);
            }
        }

        /// <summary>
        /// Visits an interpreter tree node
        /// </summary>
        /// <param name="interpreterTreeNode"></param>
        protected virtual void VisitInterpreterTreeNode(InterpreterTreeNode interpreterTreeNode)
        {
            Expression expression = interpreterTreeNode as Expression;
            if (expression != null)
            {
                VisitExpression(expression);
            }
            else
            {
                Statement.Statement statement = interpreterTreeNode as Statement.Statement;
                if (statement != null)
                {
                    VisitStatement(statement);
                }
                else
                {
                    Term term = interpreterTreeNode as Term;
                    if (term != null)
                    {
                        VisitTerm(term);
                    }
                    else
                    {
                        Designator designator = interpreterTreeNode as Designator;
                        if (designator != null)
                        {
                            VisitDesignator(designator);
                        }
                        else
                        {
                            throw new NotImplementedException();
                        }
                    }
                }
            }
        }
    }
}
