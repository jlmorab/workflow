--  Generate SQL 
--  Version:                   	V5R4M0 060210 
--  Generated on:              	14/09/15 15:18:27 
--  Relational Database:       	B103BE2F 
--  Standards Option:          	DB2 UDB iSeries 
  
SET PATH "QSYS","QSYS2","QGPL","CARVALHORA" ; 
  
CREATE PROCEDURE CARVALHORA.SEL_METODO_VALIDACION ( 
	IN P_ID_METODO INT, 
	IN P_STATUS SMALLINT ) 
	DYNAMIC RESULT SETS 1 
	LANGUAGE SQL 
	SPECIFIC CARVALHORA.SEL_METODO_VALIDACION 
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
	P1 : BEGIN 
  
		DECLARE C1 CURSOR WITH RETURN FOR 
			
			SELECT * FROM CARVALHORA . CAT_MEVA WHERE MVA_CVE = P_ID_METODO ;

		DECLARE C2 CURSOR WITH RETURN FOR 
			
			SELECT * FROM CARVALHORA . CAT_MEVA WHERE MVA_CVE = P_ID_METODO AND STA_CVE = P_STATUS ;			
						
		IF P_STATUS = -1 THEN
			
			OPEN C1 ;
			
		ELSE
			
			OPEN C2 ;			
		END IF ;
				
	END P1 ;