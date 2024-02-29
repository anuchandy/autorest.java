// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
// Code generated by Microsoft (R) AutoRest Code Generator.

package fixtures.xmlservice.models;

import com.azure.core.annotation.Fluent;
import com.azure.core.util.CoreUtils;
import com.azure.xml.XmlReader;
import com.azure.xml.XmlSerializable;
import com.azure.xml.XmlToken;
import com.azure.xml.XmlWriter;
import java.time.OffsetDateTime;
import java.time.format.DateTimeFormatter;
import javax.xml.namespace.QName;
import javax.xml.stream.XMLStreamException;

/**
 * A banana.
 */
@Fluent
public final class Banana implements XmlSerializable<Banana> {
    /*
     * The name property.
     */
    private String name;

    /*
     * The flavor property.
     */
    private String flavor;

    /*
     * The time at which you should reconsider eating this banana
     */
    private OffsetDateTime expiration;

    /**
     * Creates an instance of Banana class.
     */
    public Banana() {
    }

    /**
     * Get the name property: The name property.
     * 
     * @return the name value.
     */
    public String getName() {
        return this.name;
    }

    /**
     * Set the name property: The name property.
     * 
     * @param name the name value to set.
     * @return the Banana object itself.
     */
    public Banana setName(String name) {
        this.name = name;
        return this;
    }

    /**
     * Get the flavor property: The flavor property.
     * 
     * @return the flavor value.
     */
    public String getFlavor() {
        return this.flavor;
    }

    /**
     * Set the flavor property: The flavor property.
     * 
     * @param flavor the flavor value to set.
     * @return the Banana object itself.
     */
    public Banana setFlavor(String flavor) {
        this.flavor = flavor;
        return this;
    }

    /**
     * Get the expiration property: The time at which you should reconsider eating this banana.
     * 
     * @return the expiration value.
     */
    public OffsetDateTime getExpiration() {
        return this.expiration;
    }

    /**
     * Set the expiration property: The time at which you should reconsider eating this banana.
     * 
     * @param expiration the expiration value to set.
     * @return the Banana object itself.
     */
    public Banana setExpiration(OffsetDateTime expiration) {
        this.expiration = expiration;
        return this;
    }

    /**
     * Validates the instance.
     * 
     * @throws IllegalArgumentException thrown if the instance is not valid.
     */
    public void validate() {
    }

    @Override
    public XmlWriter toXml(XmlWriter xmlWriter) throws XMLStreamException {
        return toXml(xmlWriter, null);
    }

    @Override
    public XmlWriter toXml(XmlWriter xmlWriter, String rootElementName) throws XMLStreamException {
        rootElementName = CoreUtils.isNullOrEmpty(rootElementName) ? "banana" : rootElementName;
        xmlWriter.writeStartElement(rootElementName);
        xmlWriter.writeStringElement("name", this.name);
        xmlWriter.writeStringElement("flavor", this.flavor);
        xmlWriter.writeStringElement("expiration",
            this.expiration == null ? null : DateTimeFormatter.ISO_OFFSET_DATE_TIME.format(this.expiration));
        return xmlWriter.writeEndElement();
    }

    /**
     * Reads an instance of Banana from the XmlReader.
     * 
     * @param xmlReader The XmlReader being read.
     * @return An instance of Banana if the XmlReader was pointing to an instance of it, or null if it was pointing to XML null.
     * @throws XMLStreamException If an error occurs while reading the Banana.
     */
    public static Banana fromXml(XmlReader xmlReader) throws XMLStreamException {
        return fromXml(xmlReader, null);
    }

    /**
     * Reads an instance of Banana from the XmlReader.
     * 
     * @param xmlReader The XmlReader being read.
     * @param rootElementName Optional root element name to override the default defined by the model. Used to support cases where the model can deserialize from different root element names.
     * @return An instance of Banana if the XmlReader was pointing to an instance of it, or null if it was pointing to XML null.
     * @throws XMLStreamException If an error occurs while reading the Banana.
     */
    public static Banana fromXml(XmlReader xmlReader, String rootElementName) throws XMLStreamException {
        String finalRootElementName = CoreUtils.isNullOrEmpty(rootElementName) ? "banana" : rootElementName;
        return xmlReader.readObject(finalRootElementName, reader -> {
            Banana deserializedBanana = new Banana();
            while (reader.nextElement() != XmlToken.END_ELEMENT) {
                QName elementName = reader.getElementName();

                if ("name".equals(elementName.getLocalPart())) {
                    deserializedBanana.name = reader.getStringElement();
                } else if ("flavor".equals(elementName.getLocalPart())) {
                    deserializedBanana.flavor = reader.getStringElement();
                } else if ("expiration".equals(elementName.getLocalPart())) {
                    deserializedBanana.expiration
                        = reader.getNullableElement(dateString -> OffsetDateTime.parse(dateString));
                } else {
                    reader.skipElement();
                }
            }

            return deserializedBanana;
        });
    }
}
