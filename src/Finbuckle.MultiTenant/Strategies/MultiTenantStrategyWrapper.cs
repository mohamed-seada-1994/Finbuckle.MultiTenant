//    Copyright 2018-2020 Finbuckle LLC, Andrew White, and Contributors
//
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//
//        http://www.apache.org/licenses/LICENSE-2.0
//
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Finbuckle.MultiTenant.Strategies
{
    public class MultiTenantStrategyWrapper : IMultiTenantStrategy
    {
        public IMultiTenantStrategy Strategy { get; }

        private readonly ILogger logger;

        public MultiTenantStrategyWrapper(IMultiTenantStrategy strategy, ILogger logger)
        {
            this.Strategy = strategy ?? throw new ArgumentNullException(nameof(strategy));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<string> GetIdentifierAsync(object context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            string identifier = null;

            try
            {
                identifier = await Strategy.GetIdentifierAsync(context);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Exception in GetIdentifierAsync");
                throw new MultiTenantException($"Exception in {Strategy.GetType()}.GetIdentifierAsync.", e);
            }

            if(identifier != null)
            {
                if (logger.IsEnabled(LogLevel.Debug))
                {
                    logger.LogDebug("GetIdentifierAsync: Found identifier: \"{Identifier}\"", identifier);
                }
            }
            else
            {
                if (logger.IsEnabled(LogLevel.Debug))
                {
                    logger.LogDebug("GetIdentifierAsync: No identifier found");
                }
            }

            return identifier;
        }
    }
}
