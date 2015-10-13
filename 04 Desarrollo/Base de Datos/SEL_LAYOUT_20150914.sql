--  Generate SQL 
--  Version:                   	V5R4M0 060210 
--  Generated on:              	14/09/15 15:18:27 
--  Relational Database:       	B103BE2F 
--  Standards Option:          	DB2 UDB iSeries 
  
SET PATH "QSYS","QSYS2","QGPL","CARVALHORA" ; 
  
CREATE PROCEDURE CARVALHORA.SEL_LAYOUT ( 
	IN P_ID_LAYOUT INT ) 
	DYNAMIC RESULT SETS 1 
	LANGUAGE SQL 
	SPECIFIC CARVALHORA.SEL_LAYOUT 
	NOT DETERMINISTIC 
	READS SQL DATA 
	CALLED ON NULL INPUT 
	SET OPTION  ALWBLK = *ALLREAD , 
	ALWCPYDTA = *OPTIMIZE , 
	COMMIT = *NONE , 
	DECRESULT = (31, 31, 00) , 
	DFTRDBCOL = *NONE , 
	DYNDFTCOL = *NO , 
	DYNUSRPRF = *USER , 
	SRTSEQ = *HEX   
	BEGIN 
		P1 : BEGIN 
  
					DECLARE C1 CURSOR WITH RETURN FOR 
						SELECT * FROM CARVALHORA . CAT_LAYO WHERE LAY_CVE = P_ID_LAYOUT ;
					OPEN C1 ; 
					END P1 ; 
  
		END  ;