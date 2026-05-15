# SpacedRep

A study reminder web application built to solve a personal pain point — learning a lot in a single session, then never revisiting the material again.

SpacedRep helps you stay on top of your study material by scheduling automated revision reminders and generating quizzes from your uploaded notes using AI.

**Live app:** https://spacedrep-egax.onrender.com/

---

## The Problem

It's easy to study a topic thoroughly in one sitting and feel confident about it. The problem is that without structured follow-up, most of that knowledge fades. SpacedRep addresses this by automatically scheduling revision sessions after your initial study session, prompting you to revisit material at intervals designed to reinforce retention.

---

## Features

- Upload study material (PDF, Word, or text files)
- AI-generated quizzes based on your uploaded content
- Automated email reminders on scheduled revision dates
- Two revision scheduling strategies depending on whether you have an exam date
- Restart or regenerate revision schedules at any time

---

## How Revision Scheduling Works

SpacedRep uses two strategies depending on whether the user provides a due date.

### With a due date

When a due date is provided, the system calculates the total number of days between the study start date and the due date. It then distributes revision sessions across that window using a set of weighted intervals:

```
0.10, 0.20, 0.35, 0.55, 0.75, 0.90
```

These weights represent percentages of the total available study time. For example, with 30 days until an exam:

```
30 × 0.10 = 3 days
30 × 0.20 = 6 days
30 × 0.35 = 10.5 days
30 × 0.55 = 16.5 days
30 × 0.75 = 22.5 days
30 × 0.90 = 27 days
```

This approach scales dynamically — shorter preparation windows produce tighter revision spacing, and longer windows spread revisions further apart.

### Without a due date

When no due date is provided, the system falls back to fixed intervals inspired by common spaced repetition patterns:

```
1, 2, 4, 7, 14, 30 days
```

These are heuristic-based rather than scientifically exact. The goal is structured revision guidance that encourages repeated exposure over time, not guaranteed memorization.

Users can restart or regenerate their revision schedule at any time by updating their study start date.

---

## Tech Stack

- **Framework:** ASP.NET Core MVC (.NET 8)
- **Database:** PostgreSQL hosted on Supabase
- **ORM:** Entity Framework Core
- **Background Jobs:** Hangfire
- **File Storage:** Cloudinary
- **Email:** Brevo Transactional Email API
- **AI Quiz Generation:** Google Gemini API
- **Deployment:** Docker on Render

---

## Running Locally

**Prerequisites:**
- .NET 8 SDK
- PostgreSQL

**Setup:**

1. Clone the repository
```bash
git clone https://github.com/RodneyTakundaMadondo/SpacedRep.git
cd SpacedRep
```

2. Set up your `appsettings.json` with your local connection string:
```json
{
  "ConnectionStrings": {
    "SpacedRepAppConnectionString": "Host=localhost;Port=5432;Database=spacedrep;Username=postgres;Password=yourpassword"
  }
}
```

3. Set the following environment variables:
```
BREVO_API_KEY=your_brevo_api_key
CLOUDINARY_API_KEY=cloudinary://key:secret@cloudname
GEMINI_API_KEY=your_gemini_api_key
```

4. Run migrations:
```bash
dotnet ef database update
```

5. Run the app:
```bash
dotnet run
```

---

## Design Decisions

**Why PostgreSQL over SQL Server?** SQL Server hosting costs are prohibitive for a solo developer. PostgreSQL has a rich ecosystem of free hosting options and is production-grade.

**Why Brevo HTTP API over SMTP?** The deployment platform blocks outbound SMTP ports (25, 465, 587) on the free tier. Switching to Brevo's HTTP API over HTTPS port 443 resolved this without any infrastructure changes.

**Why weighted intervals for revision scheduling?** Rigid fixed intervals don't account for how much time a user actually has before an exam. A weighted distribution scales the revision schedule dynamically to whatever preparation window is available.

