﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ContosoUniversity.Data;
using ContosoUniversity.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ContosoUniversity.Pages.Students
{
    public class Details : PageModel
    {
        private readonly IMediator _mediator;

        public Details(IMediator mediator) => _mediator = mediator;

        public Model Data { get; private set; }

        public async Task OnGetAsync(Query query)
            => Data = await _mediator.Send(query);

        public class Query : IRequest<Model>
        {
            public int Id { get; set; }
        }

        public class Model
        {
            public int ID { get; set; }
            [Display(Name = "First Name")]
            public string FirstMidName { get; set; }
            public string LastName { get; set; }
            public DateTime EnrollmentDate { get; set; }
            public List<Enrollment> Enrollments { get; set; }

            public class Enrollment
            {
                public string CourseTitle { get; set; }
                public Grade? Grade { get; set; }
            }
        }

        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<Student, Model>();
                CreateMap<Enrollment, Model.Enrollment>();
            }
        }

        public class Handler : AsyncRequestHandler<Query, Model>
        {
            private readonly SchoolContext _db;

            public Handler(SchoolContext db) => _db = db;

            protected override async Task<Model> Handle(Query message) => await _db
                .Students
                .Include(m => m.Enrollments)
                .ThenInclude(e => e.Course)
                .Where(s => s.Id == message.Id)
                .ProjectTo<Model>()
                .SingleOrDefaultAsync();
        }
    }
}