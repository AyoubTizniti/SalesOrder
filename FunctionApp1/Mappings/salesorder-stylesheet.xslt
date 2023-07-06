<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:cdm="http://integration.team/schemas/cdm/salesorder">

  <xsl:output method="xml" version="1.0" encoding="UTF-8" indent="yes"/>

  <xsl:template match="/">
    <cdm:CDM_Order xmlns="http://integration.team/schemas/cdm/salesorder">
     <RootNode>
      <Id><xsl:value-of select="RootNode/OrderId"/></Id>
      <Date><xsl:value-of select="RootNode/OrderDate"/></Date>
      <CustomerId><xsl:value-of select="RootNode/CustomerId"/></CustomerId>
      <Name><xsl:value-of select="RootNode/CustomerName"/></Name>
      <Parties>
        <Party>
          <Function><xsl:value-of select="RootNode/Addresses[1]/Type"/></Function>
          <StreetAndNumber><xsl:value-of select="RootNode/Addresses[1]/Street"/>&#x20;<xsl:value-of select="RootNode/Addresses[1]/Number"/></StreetAndNumber>
          <PostalCode><xsl:value-of select="RootNode/Addresses[1]/Zip"/></PostalCode>
          <City><xsl:value-of select="RootNode/Addresses[1]/City"/></City>
          <Country>BE</Country>
        </Party>
      </Parties>
      <Lines>
        <xsl:for-each select="RootNode/OrderLines">
          <Line>
            <Sequence><xsl:value-of select="position()"/></Sequence>
            <Description><xsl:value-of select="Product"/></Description>
            <ProductNumber><xsl:value-of select="Id"/></ProductNumber>
            <UnitOfMeasure><xsl:value-of select="Unit"/></UnitOfMeasure>
            <Quantity><xsl:value-of select="Quantity"/></Quantity>
            <TotalPrice><xsl:value-of select="UnitPrice"/></TotalPrice>
          </Line>
        </xsl:for-each>
      </Lines>
      </RootNode>
    </cdm:CDM_Order>
  </xsl:template>

</xsl:stylesheet>
