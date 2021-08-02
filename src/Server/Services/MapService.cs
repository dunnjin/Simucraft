using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Simucraft.Server.Common;
using Simucraft.Server.Core;
using Simucraft.Server.DataAccess;
using Simucraft.Server.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MapRequest = Simucraft.Server.Core.MapRequest;

namespace Simucraft.Server.Services
{
    public class MapService : IMapService
    {
        private static readonly IList<string> SupportedMediaTypes = new List<string>
        {
            ".PNG",
            ".JPEG",
            ".JPG",
        };

        private readonly SimucraftContext _simucraftContext;
        private readonly IMapper _mapper;
        private readonly IRulesetBlobStorage _rulesetBlobStorage;

        public MapService(
            SimucraftContext simucraftContext,
            IMapper mapper,
            IRulesetBlobStorage rulesetBlobStorage)
        {
            _simucraftContext = simucraftContext;
            _mapper = mapper;
            _rulesetBlobStorage = rulesetBlobStorage;
        }

        public async Task<IEnumerable<MapResponse>> GetByRulesetIdAsync(Guid userId, Guid rulesetId)
        {
            // Ignore images when sending all maps.
            var maps = await _simucraftContext.Maps
                .Where(m => m.UserId == userId &&
                            m.RulesetId == rulesetId)
                .ToListAsync();

            var ruleset = await _simucraftContext.Rulesets.SingleOrDefaultAsync(r => r.Id == rulesetId);

            var mapResponses = _mapper.Map<IEnumerable<MapResponse>>(maps);
            foreach (var response in mapResponses)
                response.RulesetName = ruleset.Name;

            return mapResponses;
        }

        public async Task<IEnumerable<MapResponse>> GetByUserIdAsync(Guid userId)
        {
            // Ignore images when sending all maps.
            var maps = await _simucraftContext.Maps
                .Where(m => m.UserId == userId)
                .ToListAsync();

            var rulesetIds = maps
                .Select(m => m.RulesetId)
                .Distinct();

            var rulesets = await _simucraftContext.Rulesets
                .Where(r => rulesetIds.Contains(r.Id))
                .ToListAsync();

            // TODO: Find a better way to provide the parents rulesets name.
            //       Saving it on the map won't keep it updated when the ruleset name changes.
            var mapResponses = _mapper.Map<IEnumerable<MapResponse>>(maps);
            foreach (var response in mapResponses)
                response.RulesetName = rulesets.Single(r => r.Id == response.RulesetId).Name;

            return mapResponses;
        }

        public async Task<MapResponse> GetByIdAsync(Guid userId, Guid mapId)
        {
            var map = await _simucraftContext.Maps.SingleAsync(m => 
                m.Id == mapId &&
               m.UserId == userId);

            var ruleset = await _simucraftContext.Rulesets.SingleOrDefaultAsync(r => r.Id == map.RulesetId);

            var mapResponse = _mapper.Map<MapResponse>(map);
            mapResponse.RulesetName = ruleset.Name;
            //mapResponse.ImageBase64 = await _blobStorage.GetImageAsync(BLOB_CONTAINER, mapId.ToString());

            return mapResponse;
        }

        public async Task<MapResponse> AddAsync(Guid userId, Guid rulesetId, MapRequest mapRequest)
        {
            var ruleset = await _simucraftContext.Rulesets.SingleAsync(r => 
                r.Id == rulesetId &&
                r.UserId == userId);

            var currentMaps = await _simucraftContext.Maps
                .Where(m => m.RulesetId == rulesetId)
                .ToListAsync();
            if (currentMaps.Count > 5)
                throw new SubscriptionException("Only five Maps are supported at this time.");

            var map = _mapper.Map<Map>(mapRequest);
            map.Id = Guid.NewGuid();
            map.UserId = userId;
            map.RulesetId = rulesetId;

            _simucraftContext.Maps.Add(map);
            await _simucraftContext.SaveChangesAsync();

            var mapResponse = _mapper.Map<MapResponse>(map);
            mapResponse.RulesetName = ruleset.Name;

            return mapResponse;
        }

        public async Task<MapResponse> UpdateAsync(Guid userId, Guid rulesetId, Guid mapId, MapRequest mapRequest)
        {
            var ruleset = await _simucraftContext.Rulesets.SingleAsync(r => 
                r.Id == rulesetId &&
                r.UserId == userId);

            var map = _mapper.Map<Map>(mapRequest);
            map.Id = mapId;
            map.UserId = userId;
            map.RulesetId = rulesetId;

            var existingMap = await _simucraftContext.Maps.SingleAsync(m => 
                m.Id == map.Id &&
                m.UserId == userId);

            _simucraftContext.Maps.Remove(existingMap);
            _simucraftContext.Maps.Add(map);

            await _simucraftContext.SaveChangesAsync();

            var mapResponse = _mapper.Map<MapResponse>(map);
            mapResponse.RulesetName = ruleset.Name;

            return mapResponse;
        }

        public async Task DeleteAsync(Guid userId, Guid mapId)
        {
            var existingMap = await _simucraftContext.Maps.SingleAsync(m => 
                m.Id == mapId &&
                m.UserId == userId);

            await _rulesetBlobStorage.DeleteAssetAsync(existingMap.RulesetId, mapId);

            _simucraftContext.Remove(existingMap);

            await _simucraftContext.SaveChangesAsync();
        }

        public async Task SetImageAsync(Guid userId, Guid mapId, IFormFile formFile)
        {
            if (formFile.Length > ByteSize.FromMegaBytes(5))
                throw new InvalidOperationException("Image cannot exceed 5 MB.");

            var fileType = Path.GetExtension(formFile.FileName);
            if (!SupportedMediaTypes.Contains(fileType.ToUpper()))
                throw new NotSupportedException("Image type not supported.");

            var map = await _simucraftContext.Maps.SingleAsync(m => 
                m.Id == mapId &&
                m.UserId == userId);

            using (var stream = formFile.OpenReadStream())
            {
                var url = await _rulesetBlobStorage.SaveImageAsync(map.RulesetId, map.Id, stream);
                map.ImageUrl = url;
                map.ImageName = Path.GetFileName(formFile.FileName);

                using (var image = Image.FromStream(stream))
                {
                    map.Width = (int)Math.Floor((double)image.Width / map.TileWidth);
                    map.Height = (int)Math.Floor((double)image.Height / map.TileHeight);
                }
            }

            await _simucraftContext.SaveChangesAsync();
        }
    }
}
