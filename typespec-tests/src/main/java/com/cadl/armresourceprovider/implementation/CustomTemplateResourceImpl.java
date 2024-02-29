// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
// Code generated by Microsoft (R) TypeSpec Code Generator.

package com.cadl.armresourceprovider.implementation;

import com.azure.core.management.Region;
import com.azure.core.management.SystemData;
import com.azure.core.util.Context;
import com.cadl.armresourceprovider.fluent.models.CustomTemplateResourceInner;
import com.cadl.armresourceprovider.models.CustomTemplateResource;
import com.cadl.armresourceprovider.models.CustomTemplateResourceProperties;
import java.util.Collections;
import java.util.Map;

public final class CustomTemplateResourceImpl
    implements CustomTemplateResource, CustomTemplateResource.Definition, CustomTemplateResource.Update {
    private CustomTemplateResourceInner innerObject;

    private final com.cadl.armresourceprovider.ArmResourceProviderManager serviceManager;

    public String id() {
        return this.innerModel().id();
    }

    public String name() {
        return this.innerModel().name();
    }

    public String type() {
        return this.innerModel().type();
    }

    public String location() {
        return this.innerModel().location();
    }

    public Map<String, String> tags() {
        Map<String, String> inner = this.innerModel().tags();
        if (inner != null) {
            return Collections.unmodifiableMap(inner);
        } else {
            return Collections.emptyMap();
        }
    }

    public CustomTemplateResourceProperties properties() {
        return this.innerModel().properties();
    }

    public SystemData systemData() {
        return this.innerModel().systemData();
    }

    public Region region() {
        return Region.fromName(this.regionName());
    }

    public String regionName() {
        return this.location();
    }

    public String resourceGroupName() {
        return resourceGroupName;
    }

    public CustomTemplateResourceInner innerModel() {
        return this.innerObject;
    }

    private com.cadl.armresourceprovider.ArmResourceProviderManager manager() {
        return this.serviceManager;
    }

    private String resourceGroupName;

    private String customTemplateResourceName;

    public CustomTemplateResourceImpl withExistingResourceGroup(String resourceGroupName) {
        this.resourceGroupName = resourceGroupName;
        return this;
    }

    public CustomTemplateResource create() {
        this.innerObject = serviceManager.serviceClient()
            .getCustomTemplateResourceInterfaces()
            .createOrUpdate(resourceGroupName, customTemplateResourceName, this.innerModel(), Context.NONE);
        return this;
    }

    public CustomTemplateResource create(Context context) {
        this.innerObject = serviceManager.serviceClient()
            .getCustomTemplateResourceInterfaces()
            .createOrUpdate(resourceGroupName, customTemplateResourceName, this.innerModel(), context);
        return this;
    }

    CustomTemplateResourceImpl(String name, com.cadl.armresourceprovider.ArmResourceProviderManager serviceManager) {
        this.innerObject = new CustomTemplateResourceInner();
        this.serviceManager = serviceManager;
        this.customTemplateResourceName = name;
    }

    public CustomTemplateResourceImpl update() {
        return this;
    }

    public CustomTemplateResource apply() {
        this.innerObject = serviceManager.serviceClient()
            .getCustomTemplateResourceInterfaces()
            .updateWithResponse(resourceGroupName, customTemplateResourceName, this.innerModel(), Context.NONE)
            .getValue();
        return this;
    }

    public CustomTemplateResource apply(Context context) {
        this.innerObject = serviceManager.serviceClient()
            .getCustomTemplateResourceInterfaces()
            .updateWithResponse(resourceGroupName, customTemplateResourceName, this.innerModel(), context)
            .getValue();
        return this;
    }

    CustomTemplateResourceImpl(CustomTemplateResourceInner innerObject,
        com.cadl.armresourceprovider.ArmResourceProviderManager serviceManager) {
        this.innerObject = innerObject;
        this.serviceManager = serviceManager;
        this.resourceGroupName = ResourceManagerUtils.getValueFromIdByName(innerObject.id(), "resourceGroups");
        this.customTemplateResourceName
            = ResourceManagerUtils.getValueFromIdByName(innerObject.id(), "customTemplateResources");
    }

    public CustomTemplateResourceImpl withRegion(Region location) {
        this.innerModel().withLocation(location.toString());
        return this;
    }

    public CustomTemplateResourceImpl withRegion(String location) {
        this.innerModel().withLocation(location);
        return this;
    }

    public CustomTemplateResourceImpl withTags(Map<String, String> tags) {
        this.innerModel().withTags(tags);
        return this;
    }

    public CustomTemplateResourceImpl withProperties(CustomTemplateResourceProperties properties) {
        this.innerModel().withProperties(properties);
        return this;
    }
}
