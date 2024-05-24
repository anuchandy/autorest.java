// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
// Code generated by Microsoft (R) TypeSpec Code Generator.

package com.cadl.specialheaders.implementation;

import com.cadl.specialheaders.models.Resource;

/**
 * This is the Helper class to enable json merge patch serialization for a model.
 */
public class JsonMergePatchHelper {
    private static ResourceAccessor resourceAccessor;

    public interface ResourceAccessor {
        Resource prepareModelForJsonMergePatch(Resource resource, boolean jsonMergePatchEnabled);

        boolean isJsonMergePatch(Resource resource);
    }

    public static void setResourceAccessor(ResourceAccessor accessor) {
        resourceAccessor = accessor;
    }

    public static ResourceAccessor getResourceAccessor() {
        return resourceAccessor;
    }
}
