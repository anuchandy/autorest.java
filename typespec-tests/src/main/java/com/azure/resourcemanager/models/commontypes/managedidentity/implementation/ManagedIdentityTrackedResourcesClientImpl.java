// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
// Code generated by Microsoft (R) TypeSpec Code Generator.

package com.azure.resourcemanager.models.commontypes.managedidentity.implementation;

import com.azure.core.annotation.BodyParam;
import com.azure.core.annotation.ExpectedResponses;
import com.azure.core.annotation.Get;
import com.azure.core.annotation.HeaderParam;
import com.azure.core.annotation.Headers;
import com.azure.core.annotation.Host;
import com.azure.core.annotation.HostParam;
import com.azure.core.annotation.Patch;
import com.azure.core.annotation.PathParam;
import com.azure.core.annotation.Put;
import com.azure.core.annotation.QueryParam;
import com.azure.core.annotation.ReturnType;
import com.azure.core.annotation.ServiceInterface;
import com.azure.core.annotation.ServiceMethod;
import com.azure.core.annotation.UnexpectedResponseExceptionType;
import com.azure.core.http.rest.Response;
import com.azure.core.http.rest.RestProxy;
import com.azure.core.management.exception.ManagementException;
import com.azure.core.util.Context;
import com.azure.core.util.FluxUtil;
import com.azure.resourcemanager.models.commontypes.managedidentity.fluent.ManagedIdentityTrackedResourcesClient;
import com.azure.resourcemanager.models.commontypes.managedidentity.fluent.models.ManagedIdentityTrackedResourceInner;
import reactor.core.publisher.Mono;

/**
 * An instance of this class provides access to all the operations defined in ManagedIdentityTrackedResourcesClient.
 */
public final class ManagedIdentityTrackedResourcesClientImpl implements ManagedIdentityTrackedResourcesClient {
    /**
     * The proxy service used to perform REST calls.
     */
    private final ManagedIdentityTrackedResourcesService service;

    /**
     * The service client containing this operation class.
     */
    private final ManagedIdentityClientImpl client;

    /**
     * Initializes an instance of ManagedIdentityTrackedResourcesClientImpl.
     * 
     * @param client the instance of the service client containing this operation class.
     */
    ManagedIdentityTrackedResourcesClientImpl(ManagedIdentityClientImpl client) {
        this.service = RestProxy.create(ManagedIdentityTrackedResourcesService.class, client.getHttpPipeline(),
            client.getSerializerAdapter());
        this.client = client;
    }

    /**
     * The interface defining all the services for ManagedIdentityClientManagedIdentityTrackedResources to be used by
     * the proxy service to perform REST calls.
     */
    @Host("{endpoint}")
    @ServiceInterface(name = "ManagedIdentityClien")
    public interface ManagedIdentityTrackedResourcesService {
        @Headers({ "Content-Type: application/json" })
        @Get("/subscriptions/{subscriptionId}/resourceGroups/{resourceGroupName}/providers/Azure.ResourceManager.Models.CommonTypes.ManagedIdentity/managedIdentityTrackedResources/{managedIdentityTrackedResourceName}")
        @ExpectedResponses({ 200 })
        @UnexpectedResponseExceptionType(ManagementException.class)
        Mono<Response<ManagedIdentityTrackedResourceInner>> getByResourceGroup(@HostParam("endpoint") String endpoint,
            @QueryParam("api-version") String apiVersion, @PathParam("subscriptionId") String subscriptionId,
            @PathParam("resourceGroupName") String resourceGroupName,
            @PathParam("managedIdentityTrackedResourceName") String managedIdentityTrackedResourceName,
            @HeaderParam("accept") String accept, Context context);

        @Headers({ "Content-Type: application/json" })
        @Put("/subscriptions/{subscriptionId}/resourceGroups/{resourceGroupName}/providers/Azure.ResourceManager.Models.CommonTypes.ManagedIdentity/managedIdentityTrackedResources/{managedIdentityTrackedResourceName}")
        @ExpectedResponses({ 200, 201 })
        @UnexpectedResponseExceptionType(ManagementException.class)
        Mono<Response<ManagedIdentityTrackedResourceInner>> createWithSystemAssigned(
            @HostParam("endpoint") String endpoint, @QueryParam("api-version") String apiVersion,
            @PathParam("subscriptionId") String subscriptionId,
            @PathParam("resourceGroupName") String resourceGroupName,
            @PathParam("managedIdentityTrackedResourceName") String managedIdentityTrackedResourceName,
            @HeaderParam("accept") String accept,
            @BodyParam("application/json") ManagedIdentityTrackedResourceInner resource, Context context);

        @Headers({ "Content-Type: application/json" })
        @Patch("/subscriptions/{subscriptionId}/resourceGroups/{resourceGroupName}/providers/Azure.ResourceManager.Models.CommonTypes.ManagedIdentity/managedIdentityTrackedResources/{managedIdentityTrackedResourceName}")
        @ExpectedResponses({ 200 })
        @UnexpectedResponseExceptionType(ManagementException.class)
        Mono<Response<ManagedIdentityTrackedResourceInner>> updateWithUserAssignedAndSystemAssigned(
            @HostParam("endpoint") String endpoint, @QueryParam("api-version") String apiVersion,
            @PathParam("subscriptionId") String subscriptionId,
            @PathParam("resourceGroupName") String resourceGroupName,
            @PathParam("managedIdentityTrackedResourceName") String managedIdentityTrackedResourceName,
            @HeaderParam("accept") String accept,
            @BodyParam("application/json") ManagedIdentityTrackedResourceInner properties, Context context);
    }

    /**
     * Get a ManagedIdentityTrackedResource.
     * 
     * @param resourceGroupName The name of the resource group. The name is case insensitive.
     * @param managedIdentityTrackedResourceName arm resource name for path.
     * @throws IllegalArgumentException thrown if parameters fail the validation.
     * @throws ManagementException thrown if the request is rejected by server.
     * @throws RuntimeException all other wrapped checked exceptions if the request fails to be sent.
     * @return a ManagedIdentityTrackedResource along with {@link Response} on successful completion of {@link Mono}.
     */
    @ServiceMethod(returns = ReturnType.SINGLE)
    private Mono<Response<ManagedIdentityTrackedResourceInner>>
        getByResourceGroupWithResponseAsync(String resourceGroupName, String managedIdentityTrackedResourceName) {
        if (this.client.getEndpoint() == null) {
            return Mono.error(
                new IllegalArgumentException("Parameter this.client.getEndpoint() is required and cannot be null."));
        }
        if (this.client.getSubscriptionId() == null) {
            return Mono.error(new IllegalArgumentException(
                "Parameter this.client.getSubscriptionId() is required and cannot be null."));
        }
        if (resourceGroupName == null) {
            return Mono
                .error(new IllegalArgumentException("Parameter resourceGroupName is required and cannot be null."));
        }
        if (managedIdentityTrackedResourceName == null) {
            return Mono.error(new IllegalArgumentException(
                "Parameter managedIdentityTrackedResourceName is required and cannot be null."));
        }
        final String accept = "application/json";
        return FluxUtil
            .withContext(context -> service.getByResourceGroup(this.client.getEndpoint(), this.client.getApiVersion(),
                this.client.getSubscriptionId(), resourceGroupName, managedIdentityTrackedResourceName, accept,
                context))
            .contextWrite(context -> context.putAll(FluxUtil.toReactorContext(this.client.getContext()).readOnly()));
    }

    /**
     * Get a ManagedIdentityTrackedResource.
     * 
     * @param resourceGroupName The name of the resource group. The name is case insensitive.
     * @param managedIdentityTrackedResourceName arm resource name for path.
     * @param context The context to associate with this operation.
     * @throws IllegalArgumentException thrown if parameters fail the validation.
     * @throws ManagementException thrown if the request is rejected by server.
     * @throws RuntimeException all other wrapped checked exceptions if the request fails to be sent.
     * @return a ManagedIdentityTrackedResource along with {@link Response} on successful completion of {@link Mono}.
     */
    @ServiceMethod(returns = ReturnType.SINGLE)
    private Mono<Response<ManagedIdentityTrackedResourceInner>> getByResourceGroupWithResponseAsync(
        String resourceGroupName, String managedIdentityTrackedResourceName, Context context) {
        if (this.client.getEndpoint() == null) {
            return Mono.error(
                new IllegalArgumentException("Parameter this.client.getEndpoint() is required and cannot be null."));
        }
        if (this.client.getSubscriptionId() == null) {
            return Mono.error(new IllegalArgumentException(
                "Parameter this.client.getSubscriptionId() is required and cannot be null."));
        }
        if (resourceGroupName == null) {
            return Mono
                .error(new IllegalArgumentException("Parameter resourceGroupName is required and cannot be null."));
        }
        if (managedIdentityTrackedResourceName == null) {
            return Mono.error(new IllegalArgumentException(
                "Parameter managedIdentityTrackedResourceName is required and cannot be null."));
        }
        final String accept = "application/json";
        context = this.client.mergeContext(context);
        return service.getByResourceGroup(this.client.getEndpoint(), this.client.getApiVersion(),
            this.client.getSubscriptionId(), resourceGroupName, managedIdentityTrackedResourceName, accept, context);
    }

    /**
     * Get a ManagedIdentityTrackedResource.
     * 
     * @param resourceGroupName The name of the resource group. The name is case insensitive.
     * @param managedIdentityTrackedResourceName arm resource name for path.
     * @throws IllegalArgumentException thrown if parameters fail the validation.
     * @throws ManagementException thrown if the request is rejected by server.
     * @throws RuntimeException all other wrapped checked exceptions if the request fails to be sent.
     * @return a ManagedIdentityTrackedResource on successful completion of {@link Mono}.
     */
    @ServiceMethod(returns = ReturnType.SINGLE)
    private Mono<ManagedIdentityTrackedResourceInner> getByResourceGroupAsync(String resourceGroupName,
        String managedIdentityTrackedResourceName) {
        return getByResourceGroupWithResponseAsync(resourceGroupName, managedIdentityTrackedResourceName)
            .flatMap(res -> Mono.justOrEmpty(res.getValue()));
    }

    /**
     * Get a ManagedIdentityTrackedResource.
     * 
     * @param resourceGroupName The name of the resource group. The name is case insensitive.
     * @param managedIdentityTrackedResourceName arm resource name for path.
     * @param context The context to associate with this operation.
     * @throws IllegalArgumentException thrown if parameters fail the validation.
     * @throws ManagementException thrown if the request is rejected by server.
     * @throws RuntimeException all other wrapped checked exceptions if the request fails to be sent.
     * @return a ManagedIdentityTrackedResource along with {@link Response}.
     */
    @ServiceMethod(returns = ReturnType.SINGLE)
    public Response<ManagedIdentityTrackedResourceInner> getByResourceGroupWithResponse(String resourceGroupName,
        String managedIdentityTrackedResourceName, Context context) {
        return getByResourceGroupWithResponseAsync(resourceGroupName, managedIdentityTrackedResourceName, context)
            .block();
    }

    /**
     * Get a ManagedIdentityTrackedResource.
     * 
     * @param resourceGroupName The name of the resource group. The name is case insensitive.
     * @param managedIdentityTrackedResourceName arm resource name for path.
     * @throws IllegalArgumentException thrown if parameters fail the validation.
     * @throws ManagementException thrown if the request is rejected by server.
     * @throws RuntimeException all other wrapped checked exceptions if the request fails to be sent.
     * @return a ManagedIdentityTrackedResource.
     */
    @ServiceMethod(returns = ReturnType.SINGLE)
    public ManagedIdentityTrackedResourceInner getByResourceGroup(String resourceGroupName,
        String managedIdentityTrackedResourceName) {
        return getByResourceGroupWithResponse(resourceGroupName, managedIdentityTrackedResourceName, Context.NONE)
            .getValue();
    }

    /**
     * Create a ManagedIdentityTrackedResource.
     * 
     * @param resourceGroupName The name of the resource group. The name is case insensitive.
     * @param managedIdentityTrackedResourceName arm resource name for path.
     * @param resource Resource create parameters.
     * @throws IllegalArgumentException thrown if parameters fail the validation.
     * @throws ManagementException thrown if the request is rejected by server.
     * @throws RuntimeException all other wrapped checked exceptions if the request fails to be sent.
     * @return concrete tracked resource types can be created by aliasing this type using a specific property type along
     * with {@link Response} on successful completion of {@link Mono}.
     */
    @ServiceMethod(returns = ReturnType.SINGLE)
    private Mono<Response<ManagedIdentityTrackedResourceInner>> createWithSystemAssignedWithResponseAsync(
        String resourceGroupName, String managedIdentityTrackedResourceName,
        ManagedIdentityTrackedResourceInner resource) {
        if (this.client.getEndpoint() == null) {
            return Mono.error(
                new IllegalArgumentException("Parameter this.client.getEndpoint() is required and cannot be null."));
        }
        if (this.client.getSubscriptionId() == null) {
            return Mono.error(new IllegalArgumentException(
                "Parameter this.client.getSubscriptionId() is required and cannot be null."));
        }
        if (resourceGroupName == null) {
            return Mono
                .error(new IllegalArgumentException("Parameter resourceGroupName is required and cannot be null."));
        }
        if (managedIdentityTrackedResourceName == null) {
            return Mono.error(new IllegalArgumentException(
                "Parameter managedIdentityTrackedResourceName is required and cannot be null."));
        }
        if (resource == null) {
            return Mono.error(new IllegalArgumentException("Parameter resource is required and cannot be null."));
        } else {
            resource.validate();
        }
        final String accept = "application/json";
        return FluxUtil
            .withContext(context -> service.createWithSystemAssigned(this.client.getEndpoint(),
                this.client.getApiVersion(), this.client.getSubscriptionId(), resourceGroupName,
                managedIdentityTrackedResourceName, accept, resource, context))
            .contextWrite(context -> context.putAll(FluxUtil.toReactorContext(this.client.getContext()).readOnly()));
    }

    /**
     * Create a ManagedIdentityTrackedResource.
     * 
     * @param resourceGroupName The name of the resource group. The name is case insensitive.
     * @param managedIdentityTrackedResourceName arm resource name for path.
     * @param resource Resource create parameters.
     * @param context The context to associate with this operation.
     * @throws IllegalArgumentException thrown if parameters fail the validation.
     * @throws ManagementException thrown if the request is rejected by server.
     * @throws RuntimeException all other wrapped checked exceptions if the request fails to be sent.
     * @return concrete tracked resource types can be created by aliasing this type using a specific property type along
     * with {@link Response} on successful completion of {@link Mono}.
     */
    @ServiceMethod(returns = ReturnType.SINGLE)
    private Mono<Response<ManagedIdentityTrackedResourceInner>> createWithSystemAssignedWithResponseAsync(
        String resourceGroupName, String managedIdentityTrackedResourceName,
        ManagedIdentityTrackedResourceInner resource, Context context) {
        if (this.client.getEndpoint() == null) {
            return Mono.error(
                new IllegalArgumentException("Parameter this.client.getEndpoint() is required and cannot be null."));
        }
        if (this.client.getSubscriptionId() == null) {
            return Mono.error(new IllegalArgumentException(
                "Parameter this.client.getSubscriptionId() is required and cannot be null."));
        }
        if (resourceGroupName == null) {
            return Mono
                .error(new IllegalArgumentException("Parameter resourceGroupName is required and cannot be null."));
        }
        if (managedIdentityTrackedResourceName == null) {
            return Mono.error(new IllegalArgumentException(
                "Parameter managedIdentityTrackedResourceName is required and cannot be null."));
        }
        if (resource == null) {
            return Mono.error(new IllegalArgumentException("Parameter resource is required and cannot be null."));
        } else {
            resource.validate();
        }
        final String accept = "application/json";
        context = this.client.mergeContext(context);
        return service.createWithSystemAssigned(this.client.getEndpoint(), this.client.getApiVersion(),
            this.client.getSubscriptionId(), resourceGroupName, managedIdentityTrackedResourceName, accept, resource,
            context);
    }

    /**
     * Create a ManagedIdentityTrackedResource.
     * 
     * @param resourceGroupName The name of the resource group. The name is case insensitive.
     * @param managedIdentityTrackedResourceName arm resource name for path.
     * @param resource Resource create parameters.
     * @throws IllegalArgumentException thrown if parameters fail the validation.
     * @throws ManagementException thrown if the request is rejected by server.
     * @throws RuntimeException all other wrapped checked exceptions if the request fails to be sent.
     * @return concrete tracked resource types can be created by aliasing this type using a specific property type on
     * successful completion of {@link Mono}.
     */
    @ServiceMethod(returns = ReturnType.SINGLE)
    private Mono<ManagedIdentityTrackedResourceInner> createWithSystemAssignedAsync(String resourceGroupName,
        String managedIdentityTrackedResourceName, ManagedIdentityTrackedResourceInner resource) {
        return createWithSystemAssignedWithResponseAsync(resourceGroupName, managedIdentityTrackedResourceName,
            resource).flatMap(res -> Mono.justOrEmpty(res.getValue()));
    }

    /**
     * Create a ManagedIdentityTrackedResource.
     * 
     * @param resourceGroupName The name of the resource group. The name is case insensitive.
     * @param managedIdentityTrackedResourceName arm resource name for path.
     * @param resource Resource create parameters.
     * @param context The context to associate with this operation.
     * @throws IllegalArgumentException thrown if parameters fail the validation.
     * @throws ManagementException thrown if the request is rejected by server.
     * @throws RuntimeException all other wrapped checked exceptions if the request fails to be sent.
     * @return concrete tracked resource types can be created by aliasing this type using a specific property type along
     * with {@link Response}.
     */
    @ServiceMethod(returns = ReturnType.SINGLE)
    public Response<ManagedIdentityTrackedResourceInner> createWithSystemAssignedWithResponse(String resourceGroupName,
        String managedIdentityTrackedResourceName, ManagedIdentityTrackedResourceInner resource, Context context) {
        return createWithSystemAssignedWithResponseAsync(resourceGroupName, managedIdentityTrackedResourceName,
            resource, context).block();
    }

    /**
     * Create a ManagedIdentityTrackedResource.
     * 
     * @param resourceGroupName The name of the resource group. The name is case insensitive.
     * @param managedIdentityTrackedResourceName arm resource name for path.
     * @param resource Resource create parameters.
     * @throws IllegalArgumentException thrown if parameters fail the validation.
     * @throws ManagementException thrown if the request is rejected by server.
     * @throws RuntimeException all other wrapped checked exceptions if the request fails to be sent.
     * @return concrete tracked resource types can be created by aliasing this type using a specific property type.
     */
    @ServiceMethod(returns = ReturnType.SINGLE)
    public ManagedIdentityTrackedResourceInner createWithSystemAssigned(String resourceGroupName,
        String managedIdentityTrackedResourceName, ManagedIdentityTrackedResourceInner resource) {
        return createWithSystemAssignedWithResponse(resourceGroupName, managedIdentityTrackedResourceName, resource,
            Context.NONE).getValue();
    }

    /**
     * Update a ManagedIdentityTrackedResource.
     * 
     * @param resourceGroupName The name of the resource group. The name is case insensitive.
     * @param managedIdentityTrackedResourceName arm resource name for path.
     * @param properties The resource properties to be updated.
     * @throws IllegalArgumentException thrown if parameters fail the validation.
     * @throws ManagementException thrown if the request is rejected by server.
     * @throws RuntimeException all other wrapped checked exceptions if the request fails to be sent.
     * @return concrete tracked resource types can be created by aliasing this type using a specific property type along
     * with {@link Response} on successful completion of {@link Mono}.
     */
    @ServiceMethod(returns = ReturnType.SINGLE)
    private Mono<Response<ManagedIdentityTrackedResourceInner>>
        updateWithUserAssignedAndSystemAssignedWithResponseAsync(String resourceGroupName,
            String managedIdentityTrackedResourceName, ManagedIdentityTrackedResourceInner properties) {
        if (this.client.getEndpoint() == null) {
            return Mono.error(
                new IllegalArgumentException("Parameter this.client.getEndpoint() is required and cannot be null."));
        }
        if (this.client.getSubscriptionId() == null) {
            return Mono.error(new IllegalArgumentException(
                "Parameter this.client.getSubscriptionId() is required and cannot be null."));
        }
        if (resourceGroupName == null) {
            return Mono
                .error(new IllegalArgumentException("Parameter resourceGroupName is required and cannot be null."));
        }
        if (managedIdentityTrackedResourceName == null) {
            return Mono.error(new IllegalArgumentException(
                "Parameter managedIdentityTrackedResourceName is required and cannot be null."));
        }
        if (properties == null) {
            return Mono.error(new IllegalArgumentException("Parameter properties is required and cannot be null."));
        } else {
            properties.validate();
        }
        final String accept = "application/json";
        return FluxUtil
            .withContext(context -> service.updateWithUserAssignedAndSystemAssigned(this.client.getEndpoint(),
                this.client.getApiVersion(), this.client.getSubscriptionId(), resourceGroupName,
                managedIdentityTrackedResourceName, accept, properties, context))
            .contextWrite(context -> context.putAll(FluxUtil.toReactorContext(this.client.getContext()).readOnly()));
    }

    /**
     * Update a ManagedIdentityTrackedResource.
     * 
     * @param resourceGroupName The name of the resource group. The name is case insensitive.
     * @param managedIdentityTrackedResourceName arm resource name for path.
     * @param properties The resource properties to be updated.
     * @param context The context to associate with this operation.
     * @throws IllegalArgumentException thrown if parameters fail the validation.
     * @throws ManagementException thrown if the request is rejected by server.
     * @throws RuntimeException all other wrapped checked exceptions if the request fails to be sent.
     * @return concrete tracked resource types can be created by aliasing this type using a specific property type along
     * with {@link Response} on successful completion of {@link Mono}.
     */
    @ServiceMethod(returns = ReturnType.SINGLE)
    private Mono<Response<ManagedIdentityTrackedResourceInner>>
        updateWithUserAssignedAndSystemAssignedWithResponseAsync(String resourceGroupName,
            String managedIdentityTrackedResourceName, ManagedIdentityTrackedResourceInner properties,
            Context context) {
        if (this.client.getEndpoint() == null) {
            return Mono.error(
                new IllegalArgumentException("Parameter this.client.getEndpoint() is required and cannot be null."));
        }
        if (this.client.getSubscriptionId() == null) {
            return Mono.error(new IllegalArgumentException(
                "Parameter this.client.getSubscriptionId() is required and cannot be null."));
        }
        if (resourceGroupName == null) {
            return Mono
                .error(new IllegalArgumentException("Parameter resourceGroupName is required and cannot be null."));
        }
        if (managedIdentityTrackedResourceName == null) {
            return Mono.error(new IllegalArgumentException(
                "Parameter managedIdentityTrackedResourceName is required and cannot be null."));
        }
        if (properties == null) {
            return Mono.error(new IllegalArgumentException("Parameter properties is required and cannot be null."));
        } else {
            properties.validate();
        }
        final String accept = "application/json";
        context = this.client.mergeContext(context);
        return service.updateWithUserAssignedAndSystemAssigned(this.client.getEndpoint(), this.client.getApiVersion(),
            this.client.getSubscriptionId(), resourceGroupName, managedIdentityTrackedResourceName, accept, properties,
            context);
    }

    /**
     * Update a ManagedIdentityTrackedResource.
     * 
     * @param resourceGroupName The name of the resource group. The name is case insensitive.
     * @param managedIdentityTrackedResourceName arm resource name for path.
     * @param properties The resource properties to be updated.
     * @throws IllegalArgumentException thrown if parameters fail the validation.
     * @throws ManagementException thrown if the request is rejected by server.
     * @throws RuntimeException all other wrapped checked exceptions if the request fails to be sent.
     * @return concrete tracked resource types can be created by aliasing this type using a specific property type on
     * successful completion of {@link Mono}.
     */
    @ServiceMethod(returns = ReturnType.SINGLE)
    private Mono<ManagedIdentityTrackedResourceInner> updateWithUserAssignedAndSystemAssignedAsync(
        String resourceGroupName, String managedIdentityTrackedResourceName,
        ManagedIdentityTrackedResourceInner properties) {
        return updateWithUserAssignedAndSystemAssignedWithResponseAsync(resourceGroupName,
            managedIdentityTrackedResourceName, properties).flatMap(res -> Mono.justOrEmpty(res.getValue()));
    }

    /**
     * Update a ManagedIdentityTrackedResource.
     * 
     * @param resourceGroupName The name of the resource group. The name is case insensitive.
     * @param managedIdentityTrackedResourceName arm resource name for path.
     * @param properties The resource properties to be updated.
     * @param context The context to associate with this operation.
     * @throws IllegalArgumentException thrown if parameters fail the validation.
     * @throws ManagementException thrown if the request is rejected by server.
     * @throws RuntimeException all other wrapped checked exceptions if the request fails to be sent.
     * @return concrete tracked resource types can be created by aliasing this type using a specific property type along
     * with {@link Response}.
     */
    @ServiceMethod(returns = ReturnType.SINGLE)
    public Response<ManagedIdentityTrackedResourceInner> updateWithUserAssignedAndSystemAssignedWithResponse(
        String resourceGroupName, String managedIdentityTrackedResourceName,
        ManagedIdentityTrackedResourceInner properties, Context context) {
        return updateWithUserAssignedAndSystemAssignedWithResponseAsync(resourceGroupName,
            managedIdentityTrackedResourceName, properties, context).block();
    }

    /**
     * Update a ManagedIdentityTrackedResource.
     * 
     * @param resourceGroupName The name of the resource group. The name is case insensitive.
     * @param managedIdentityTrackedResourceName arm resource name for path.
     * @param properties The resource properties to be updated.
     * @throws IllegalArgumentException thrown if parameters fail the validation.
     * @throws ManagementException thrown if the request is rejected by server.
     * @throws RuntimeException all other wrapped checked exceptions if the request fails to be sent.
     * @return concrete tracked resource types can be created by aliasing this type using a specific property type.
     */
    @ServiceMethod(returns = ReturnType.SINGLE)
    public ManagedIdentityTrackedResourceInner updateWithUserAssignedAndSystemAssigned(String resourceGroupName,
        String managedIdentityTrackedResourceName, ManagedIdentityTrackedResourceInner properties) {
        return updateWithUserAssignedAndSystemAssignedWithResponse(resourceGroupName,
            managedIdentityTrackedResourceName, properties, Context.NONE).getValue();
    }
}
