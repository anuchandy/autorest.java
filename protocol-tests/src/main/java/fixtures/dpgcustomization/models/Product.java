// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
// Code generated by Microsoft (R) AutoRest Code Generator.

package fixtures.dpgcustomization.models;

import com.azure.core.annotation.Generated;
import com.azure.core.annotation.Immutable;
import com.azure.json.JsonReader;
import com.azure.json.JsonSerializable;
import com.azure.json.JsonToken;
import com.azure.json.JsonWriter;
import java.io.IOException;

/**
 * The Product model.
 */
@Immutable
public class Product implements JsonSerializable<Product> {
    /*
     * The received property.
     */
    @Generated
    private final ProductReceived received;

    /**
     * Creates an instance of Product class.
     * 
     * @param received the received value to set.
     */
    @Generated
    public Product(ProductReceived received) {
        this.received = received;
    }

    /**
     * Get the received property: The received property.
     * 
     * @return the received value.
     */
    @Generated
    public ProductReceived getReceived() {
        return this.received;
    }

    /**
     * {@inheritDoc}
     */
    @Generated
    @Override
    public JsonWriter toJson(JsonWriter jsonWriter) throws IOException {
        jsonWriter.writeStartObject();
        jsonWriter.writeStringField("received", this.received == null ? null : this.received.toString());
        return jsonWriter.writeEndObject();
    }

    /**
     * Reads an instance of Product from the JsonReader.
     * 
     * @param jsonReader The JsonReader being read.
     * @return An instance of Product if the JsonReader was pointing to an instance of it, or null if it was pointing to JSON null.
     * @throws IllegalStateException If the deserialized JSON object was missing any required properties.
     * @throws IOException If an error occurs while reading the Product.
     */
    @Generated
    public static Product fromJson(JsonReader jsonReader) throws IOException {
        return jsonReader.readObject(reader -> {
            boolean receivedFound = false;
            ProductReceived received = null;
            while (reader.nextToken() != JsonToken.END_OBJECT) {
                String fieldName = reader.getFieldName();
                reader.nextToken();

                if ("received".equals(fieldName)) {
                    received = ProductReceived.fromString(reader.getString());
                    receivedFound = true;
                } else {
                    reader.skipChildren();
                }
            }
            if (receivedFound) {
                return new Product(received);
            }
            throw new IllegalStateException("Missing required property: received");
        });
    }
}
