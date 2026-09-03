using KerlaVlogs.Data;
using KerlaVlogs.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// MVC
builder.Services.AddControllersWithViews();

// ASP.NET Core Identity
builder.Services
    .AddDefaultIdentity<IdentityUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider
        .GetRequiredService<UserManager<IdentityUser>>();

    var adminEmail = Environment.GetEnvironmentVariable("KERLA_ADMIN_EMAIL");
    var adminPassword = Environment.GetEnvironmentVariable("KERLA_ADMIN_PASSWORD");

    if (string.IsNullOrWhiteSpace(adminEmail) ||
        string.IsNullOrWhiteSpace(adminPassword))
    {
        throw new InvalidOperationException(
            "Admin credentials are not configured.");
    }

    var adminUser = await userManager.FindByEmailAsync(adminEmail);

    if (adminUser == null)
    {
        adminUser = new IdentityUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(
            adminUser,
            adminPassword);

        if (!result.Succeeded)
        {
            var errors = string.Join(
                ", ",
                result.Errors.Select(e => e.Description));

            throw new InvalidOperationException(
                $"Admin user creation failed: {errors}");
        }
    }
}

// Create database scope
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider
        .GetRequiredService<ApplicationDbContext>();


    // =========================
    // DESTINATIONS SEED
    // =========================

    if (!context.Destinations.Any())
    {
        context.Destinations.AddRange(

            new Destination
            {
                Name = "Munnar",
                Description = "Munnar was one of the most beautiful places of our Kerala journey. The tea gardens, mountains and pleasant weather made the experience unforgettable.",
                Image = "/images/munnar.jpeg",
                Location = "Idukki, Kerala",
                PlacesVisited = "Tea Gardens, Waterfalls, View Points"
            },

            new Destination
            {
                Name = "Alleppey",
                Description = "Alleppey was a peaceful and beautiful experience. The famous backwaters and houseboats made this part of our journey very special.",
                Image = "/images/alleppey.jpeg",
                Location = "Alappuzha, Kerala",
                PlacesVisited = "Backwaters, Houseboat, Sunset"
            },

            new Destination
            {
                Name = "Varkala",
                Description = "Varkala gave us a completely different experience with its beautiful beach, cliffs and amazing sunset views.",
                Image = "/images/varkala.jpeg",
                Location = "Thiruvananthapuram, Kerala",
                PlacesVisited = "Varkala Beach, Cliff, Sunset Point"
            }
        );

        context.SaveChanges();
    }


    // =========================
    // ITINERARY SEED
    // =========================

    if (!context.Itineraries.Any())
    {
        context.Itineraries.AddRange(

            new Itinerary
            {
                DayNumber = 1,
                Title = "Mumbai → Kochi",
                Location = "Mumbai → Ernakulam, Kochi",
                Description = "Our Kerala journey began on 2nd July. We took a train from Mumbai LTT (Lokmanya Tilak Terminus) to Ernakulam Junction in Kochi. This marked the beginning of our exciting 7-day Kerala adventure."
            },

            new Itinerary
            {
                DayNumber = 2,
                Title = "Exploring Fort Kochi",
                Location = "Fort Kochi, Kerala",
                Description = "We reached Kochi on 3rd July and explored the famous Chinese Fishing Nets and Fort Kochi. We also experienced Kalaripayattu, the traditional martial art of Kerala, and Theyyam, a traditional ritualistic art form. After exploring Kochi, we had dinner and boarded a night bus towards Munnar."
            },

            new Itinerary
            {
                DayNumber = 3,
                Title = "Kochi → Munnar & Eravikulam National Park",
                Location = "Munnar, Kerala",
                Description = "We reached Munnar at around 4:00 AM and checked into our hotel. In the afternoon, we visited Eravikulam National Park. The huge tea plantations, clouds, rain and beautiful mountain views made the experience unforgettable. We were also lucky enough to see the Nilgiri Tahr, an endemic animal of the Western Ghats. Later, we returned to the hotel, enjoyed delicious biryani and had a small party."
            },

            new Itinerary
            {
                DayNumber = 4,
                Title = "Munnar Tea Gardens → Thekkady",
                Location = "Munnar → Thekkady",
                Description = "We woke up early and explored the beautiful tea gardens of Munnar. We took lots of pictures and enjoyed the stunning scenery. Later, we boarded a bus towards Thekkady. The journey through the Western Ghats was beautiful, with mountain curves and breathtaking views. We reached Thekkady in the evening, explored the local market and planned our activities for the next day."
            },

            new Itinerary
            {
                DayNumber = 5,
                Title = "Periyar Wildlife Sanctuary & Jeep Safari → Alleppey",
                Location = "Thekkady → Alleppey",
                Description = "We started early in the morning and visited Periyar Wildlife Sanctuary. We explored the Periyar Lake on a large boat and spotted several animals along the way. Surrounded by forests, mountains and the peaceful lake, it felt like a real Man vs. Wild experience. After lunch, we went on a thrilling Jeep Safari through the mountains. The off-roading experience was on another level. Later, we returned to the hotel and travelled by bus to Alleppey. The journey through the Western Ghats was stunning."
            },

            new Itinerary
            {
                DayNumber = 6,
                Title = "Alleppey Backwaters → Alappuzha Beach → Trivandrum",
                Location = "Alleppey → Thiruvananthapuram",
                Description = "We started the day by exploring the famous Alleppey backwaters by boat. We travelled through the beautiful waterways and explored Vembanad Lake, the largest lake in Kerala. The peaceful surroundings and greenery made the experience unforgettable. Later, we enjoyed a traditional Kerala Sadya served on a banana leaf and absolutely loved the food. In the evening, we explored Alappuzha Beach and then boarded a train towards Thiruvananthapuram. After reaching there, we checked into our hotel and rested."
            },

            new Itinerary
            {
                DayNumber = 7,
                Title = "Padmanabhaswamy Temple → Poovar → Azhimala → Kovalam",
                Location = "Thiruvananthapuram, Kerala",
                Description = "Day 7 was the best day of our entire Kerala journey. We started by visiting the famous Sree Padmanabhaswamy Temple. Then we headed towards Poovar Island, where we enjoyed an incredible boat ride through beautiful waterways, forests and greenery before reaching the Arabian Sea. We explored the Golden Sand Beach and several other beautiful spots. Later, we visited Azhimala and its massive Shiva statue, along with the cave containing numerous sculptures. Finally, we headed to Kovalam Beach and the Kovalam Lighthouse. The blue waters, clean beach and breathtaking view from the lighthouse made it one of the best experiences of our lives. After spending 2–3 hours there, we returned to our hotel, had a small farewell party and went to sleep. The next morning, we boarded our train back to Mumbai, carrying seven days of unforgettable memories with us."
            }
        );

        context.SaveChanges();
    }


    // =========================
    // ASSIGN DESTINATIONS
    // =========================

    var itineraryDays = context.Itineraries
        .OrderBy(x => x.DayNumber)
        .ToList();

    var munnar = context.Destinations
        .FirstOrDefault(x => x.Name == "Munnar");

    var alleppey = context.Destinations
        .FirstOrDefault(x => x.Name == "Alleppey");


    if (itineraryDays.Count >= 7)
    {
        itineraryDays[0].DestinationId = null;
        itineraryDays[1].DestinationId = null;

        itineraryDays[2].DestinationId = munnar?.Id;
        itineraryDays[3].DestinationId = munnar?.Id;

        itineraryDays[4].DestinationId = null;

        itineraryDays[5].DestinationId = alleppey?.Id;

        itineraryDays[6].DestinationId = null;

        context.SaveChanges();
    }
}


// =========================
// HTTP PIPELINE
// =========================

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();