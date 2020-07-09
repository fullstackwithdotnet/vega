﻿using AutoMapper;
using System.Linq;
using Vega.Controllers.Resoursces;
using Vega.Core.Models;

namespace Vega.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            //domain to resource
            CreateMap(typeof(QueryResult<>), typeof(QueryResultResource<>));
            CreateMap<Make, MakeResource>();
            CreateMap<Make, KeyValuePairResource>();
            CreateMap<Model, KeyValuePairResource>();
            CreateMap<Feature, KeyValuePairResource>();
            CreateMap<Vehicle, SaveVehicleRecource>()
                .ForMember(vr => vr.Contact, opt => opt.MapFrom(v => new ContactResource { Name = v.ContactName, Email = v.ContactEmail, Phone = v.ContactPhone }))
                .ForMember(vr => vr.Features, opt => opt.MapFrom(v => v.Features.Select(f => f.FeatureId)));
            

            CreateMap<Vehicle, VehicleRecource>()
                 .ForMember(vr => vr.Contact, opt => opt.MapFrom(v => new ContactResource { Name = v.ContactName, Email = v.ContactEmail, Phone = v.ContactPhone }))
                 .ForMember(vr => vr.Features, opt => opt.MapFrom(v => v.Features.Select(vf => new KeyValuePairResource { Id = vf.Feature.Id, Name = vf.Feature.Name })))
                 .ForMember(vr => vr.Make, opt => opt.MapFrom(v => new MakeResource { Id = v.Model.Make.Id, Name = v.Model.Make.Name }));

            //api resource ro domain

            CreateMap<VehicleQueryResource, VehicleQuery>();

            CreateMap<SaveVehicleRecource, Vehicle>()
                .ForMember(v => v.Id, opt => opt.Ignore())
                .ForMember(v => v.ContactName, opt => opt.MapFrom(vr => vr.Contact.Name))
                .ForMember(v => v.ContactEmail, opt => opt.MapFrom(vr => vr.Contact.Email))
                .ForMember(v => v.ContactPhone, opt => opt.MapFrom(vr => vr.Contact.Phone))
                .ForMember(v => v.Features, opt => opt.Ignore())
                .AfterMap((vr, v) =>
                {

                    var removedFeatures = v.Features.Where(f => !vr.Features.Contains(f.FeatureId)).ToList();


                    foreach (var f in removedFeatures)
                    {
                        v.Features.Remove(f);
                    }


                    var addedFeatures = vr.Features.Where(id => !v.Features.Any(vf => vf.FeatureId == id)).Select(id => new VehicleFeature { FeatureId = id }).ToList();

                    foreach (var f in addedFeatures)
                    {
                        v.Features.Add(f);
                    }

                });
        }
    }
}
