﻿grammar Calculator;

compileUnit : expression EOF;
expression :
LPAREN expression RPAREN #ParenthesizedExpr
|expression EXPONENT expression #ExponentialExpr
| expression operatorToken=(MULTIPLY | DIVIDE) expression #MultiplicativeExpr
| operatorToken=SUBTRACT expression #SubOper
| operatorToken=ADD expression #AddOper
| expression operatorToken=(ADD | SUBTRACT) expression #AdditiveExpr
| operatorToken=(MOD | DIV) LPAREN expression ',' expression RPAREN #ModDivExpr
| MMAX LPAREN paramlist=arglist RPAREN #MmaxExpr
| MMIN LPAREN paramlist=arglist RPAREN #MminExpr
| INC LPAREN expression RPAREN #IncExpr
| NUMBER #NumberExpr
| IDENTIFIER #IdentifierExpr
;
arglist: expression (',' expression)+;
paramlist: expression (',' expression)+;
/*
 * Lexer Rules
 */
NUMBER : INT ('.' INT)?;
IDENTIFIER : [a-zA-Z]*[1-9][0-9]*;
INT : ('0'..'9')+;
EXPONENT : '^';
MULTIPLY : '*';
DIVIDE : '/';
SUBTRACT : '-';
ADD : '+';
LPAREN : '(';
RPAREN : ')';
MOD : 'mod';
DIV : 'div';
INC : 'inc';
MMAX : 'mmax';
MMIN : 'mmin';

WS : [ \t\r\n] -> channel(HIDDEN);