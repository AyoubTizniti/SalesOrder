<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" 
	exclude-result-prefixes="msxsl"
	xmlns:extDateTime="urn:my-scripts"
>
    <xsl:output method="xml" indent="yes"/>

    <xsl:template match="/">
		<OutputResult>
			<EdiDateTime>
				<xsl:value-of select="extDateTime:EdiDateTime()"/>
			</EdiDateTime>
			<UtcDateTime>
				<xsl:value-of select="extDateTime:UtcDateTime()"/>
			</UtcDateTime>
		</OutputResult>
    </xsl:template>

	<msxsl:script language="C#" implements-prefix="extDateTime">
		<msxsl:assembly name="IT.Netic.Helpers.DateTimeFunctions" />
		<msxsl:using namespace="IT.Netic.Helpers.DateTimeFunctions" />
		<![CDATA[
			public string EdiDateTime(){ return DateTimeHelper.EdiUtcDateString(); }
			public string UtcDateTime(){ return DateTimeHelper.UtcDateNowString(); }
		]]>
	</msxsl:script>
	
	
</xsl:stylesheet>
