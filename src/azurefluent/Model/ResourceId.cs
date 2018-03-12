
using System;

namespace AutoRest.Java.Azure.Fluent.Model
{
    public sealed partial class ResourceId
    {
        private string subscriptionId = null;
        private string resourceGroupName = null;
        private string name = null;
        private string providerNamespace = null;
        private string resourceType = null;
        private string id = null;
        private string parentId = null;

        private static string badIdErrorText(string id)
        {
            return string.Format("The specified ID {0} is not a valid Azure resource ID.", id);
        }

        private ResourceId(string id)
        {
            if (id == null)
            {
                // Protect against NPEs from null IDs, preserving legacy behavior for null IDs
                return;
            }
            else
            {
                // Skip the first '/' if any, and then split using '/'
                string[] splits = (id.StartsWith("/")) ? id.Substring(1).Split('/') : id.Split('/');
                if (splits.Length % 2 == 1)
                {
                    throw new ArgumentException(badIdErrorText(id));
                }

                // Save the ID itself
                this.id = id;

                // Format of id:
                // /subscriptions/<subscriptionId>/resourceGroups/<resourceGroupName>/providers/<providerNamespace>(/<parentResourceType>/<parentName>)*/<resourceType>/<name>
                //  0             1                2              3                   4         5                                                        N-2            N-1

                // Extract resource type and name
                if (splits.Length < 2)
                {
                    throw new ArgumentException(badIdErrorText(id));
                }
                else
                {
                    name = splits[splits.Length - 1];
                    resourceType = splits[splits.Length - 2];
                }

                // Extract parent ID
                if (splits.Length < 10)
                {
                    parentId = null;
                }
                else
                {
                    string[] parentSplits = new string[splits.Length - 2];
                    Array.Copy(splits, parentSplits, splits.Length - 2);
                    parentId = "/" + string.Join("/", parentSplits);
                }

                for (int i = 0; i < splits.Length && i < 6; i++)
                {
                    switch (i)
                    {
                        case 0:
                            // Ensure "subscriptions"
                            if (string.Compare(splits[i], "subscriptions", StringComparison.OrdinalIgnoreCase) != 0)
                            {
                                throw new ArgumentException(badIdErrorText(id));
                            }
                            break;
                        case 1:
                            // Extract subscription ID
                            subscriptionId = splits[i];
                            break;
                        case 2:
                            // Ensure "resourceGroups"
                            if (string.Compare(splits[i], "resourceGroups", StringComparison.OrdinalIgnoreCase) != 0)
                            {
                                throw new ArgumentException(badIdErrorText(id));
                            }
                            break;
                        case 3:
                            // Extract resource group name
                            resourceGroupName = splits[i];
                            break;
                        case 4:
                            // Ensure "providers"
                            if (string.Compare(splits[i], "providers", StringComparison.OrdinalIgnoreCase) != 0)
                            {
                                throw new ArgumentException(badIdErrorText(id));
                            }
                            break;
                        case 5:
                            // Extract provider namespace
                            providerNamespace = splits[i];
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        public static ResourceId FromString(string id)
        {
            return new ResourceId(id);
        }

        public string SubscriptionId
        {
            get
            {
                return subscriptionId;
            }
        }

        public string ResourceGroupName
        {
            get
            {
                return resourceGroupName;
            }
        }

        public string Name
        {
            get
            {
                return name;
            }
        }

        public ResourceId Parent
        {
            get
            {
                if (id == null || parentId == null)
                {
                    return null;
                }
                else
                {
                    return FromString(parentId);
                }
            }
        }

        public string ProviderNamespace
        {
            get
            {
                return providerNamespace;
            }
        }

        public string ResourceType
        {
            get
            {
                return resourceType;
            }
        }

        public string FullResourceType
        {
            get
            {
                if (parentId == null)
                {
                    return providerNamespace + "/" + resourceType;
                }
                else
                {
                    return this.Parent.FullResourceType + "/" + resourceType;
                }
            }
        }

        public string Id
        {
            get
            {
                return id;
            }
        }
    }
}