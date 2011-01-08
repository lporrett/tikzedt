﻿using System;
using System.Collections.Generic;
using System.Text;
using Antlr.Runtime;

namespace TikzEdt.Parser
{
    /// <summary>
    /// This static class creates a human readable error string
    /// from a ANTLRException. This string can be passed to the user.
    /// </summary>
    static class ANTLRErrorMsg
    {
        static public string ToString(RecognitionException Ex, string[] tokenNames)
        {
            //Stores a string that will be displayed after the actual error message
            //to give the user a specific hint what might be wrong.
            string TikzEdtNotice = "";
            //message that will be returned.
            string msg = "";
            //Types of RecognitionException are defined here:
            //http://www.antlr.org/api/Python/classantlr3_1_1_recognition_exception.html
            if (Ex is EarlyExitException)
            {
                EarlyExitException ex = Ex as EarlyExitException;
                msg += "EarlyExitException";
            }
            else if (Ex is FailedPredicateException)
            {
                FailedPredicateException ex = Ex as FailedPredicateException;
                msg += "FailedPredicateException";
            }
            else if (Ex is MismatchedRangeException)
            {
                MismatchedRangeException ex = Ex as MismatchedRangeException;
                msg += "MismatchedRangeException";
            }
            else if (Ex is MismatchedSetException)
            {
                MismatchedSetException ex = Ex as MismatchedSetException;
                msg += "MismatchedSetException";
                if (Ex is MismatchedNotSetException)
                {
                    MismatchedNotSetException ex2 = ex as MismatchedNotSetException;
                    msg += " -> MismatchedNotSetException";
                }
            }
            else if (Ex is MismatchedTokenException)
            {
                MismatchedTokenException ex = Ex as MismatchedTokenException;
                msg += "MismatchedTokenException";
                if (Ex is MissingTokenException)
                {
                    MissingTokenException ex2 = ex as MissingTokenException;
                    msg += " -> MissingTokenException";
                }
                else if (Ex is UnwantedTokenException)
                {
                    UnwantedTokenException ex2 = ex as UnwantedTokenException;
                    msg += " -> UnwantedTokenException";
                }
                else
                {
                    msg += ": Expected token type " + tokenNames[ex.Expecting] + ".";
                    if (ex.Token.Text != null)
                    {                        
                        msg += " Instead found \"" + ex.Token.Text.Replace("\n", "<NewLine>") + "\" which is from type " + tokenNames[ex.Token.Type];                        
                    }
                    else
                    {
                        msg += " Instead found EOF";
                        TikzEdtNotice = "Does document include \\begin{tikzpicture} and \\end{tikzpicture}?";
                    }
                }
            }
            else if (Ex is MismatchedTreeNodeException)
            {
                MismatchedTreeNodeException ex = Ex as MismatchedTreeNodeException;
                msg += "MismatchedTreeNodeException";
            }
            else if (Ex is NoViableAltException)
            {
                NoViableAltException ex = Ex as NoViableAltException;
                msg += "NoViableAltException";
            }
            else
            {
                msg += "UnkownExcpetion (this is really bad)";
            }
            if (Ex.Line > 0)
                msg += " in line " + Ex.Line.ToString() + " at position " + Ex.CharPositionInLine.ToString();
            else
                msg += " at end of document";

            if (Ex.approximateLineInfo)
                msg += " (approximately)";

            msg += ".";

            if (TikzEdtNotice != "")
                msg += " " + TikzEdtNotice;

            return msg;
        }
    }
}