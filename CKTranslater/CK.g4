grammar CK;

script : record* EOF;

record : lvalue OPERATOR? rvalue;

lvalue : BOOL
	   | INT
	   | FLOAT
	   | DATE
	   | HEX
       | STRING
	   ;

rvalue : BOOL
	   | INT
	   | FLOAT
	   | DATE
	   | HEX
       | STRING
	   | block
	   ;
	   
block : LQUOTE OPERATOR? (array | record*) (RQUOTE|EOF);
	   
array :  (BOOL | STRING)+
	   | (INT | FLOAT | HEX | STRING)+
	   | (DATE | STRING)+
       | STRING+
       | block+
       ;


OPERATOR : '=' | '<' | '>' | '==' | '<=' | '>=' ;

fragment
DIGIT:  '0'..'9' ; 

INT :   ('-'|'+')? DIGIT+ ;

LQUOTE : '{';
RQUOTE : '}';

BOOL : 'yes' | 'no';

HEX :   '0' ('x'|'X') HEXDIGIT+ ;

DATE : INT '.' INT '.' INT '.'?;

fragment
HEXDIGIT : (DIGIT|'a'..'f'|'A'..'F') ;

FLOAT: ('-'|'+')? DIGIT* '.' DIGIT* ;

fragment
LETTER  : [a-zA-Z\u0080-\u00FF_] ;

COMMENT : '#' .*? (EOF|'\r'? '\n') -> channel(HIDDEN) ;

STRING : ('"' ~[{}]*? ('"'|NL))
       | (~[ \r\n\t{}=<>#]+)
       ;

fragment
NL      :   '\r'? '\n' ;

WS : [ \r\t\n]+ -> channel(HIDDEN) ;