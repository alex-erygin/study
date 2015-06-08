<?xml version="1.0" ?>
<xsl:stylesheet version="1.0"     xmlns:xsl="http://www.w3.org/1999/XSL/Transform"     xmlns:wix="http://schemas.microsoft.com/wix/2006/wi">
    <xsl:output method="xml" version="1.0" encoding="UTF-8" indent="yes"/>
    
    <!-- Adds all by default -->
    <xsl:template match="@*|*">
        <xsl:copy>
            <xsl:apply-templates select="@*" />
            <xsl:apply-templates select="*" />
        </xsl:copy>
    </xsl:template>

    <!-- Except those -->
    <!-- http://stackoverflow.com/questions/8737356/how-can-i-exclude-svn-files-from-harvesting-with-heat-wix -->
    <!-- http://habrahabr.ru/post/122038/ -->
    <xsl:template match="wix:Fragment/wix:ComponentGroup/wix:Component[substring(wix:File/@Source, string-length(wix:File/@Source) - string-length('Infotecs.LSP.Application.exe') + 1) = 'Infotecs.LSP.Application.exe']" />
</xsl:stylesheet>
