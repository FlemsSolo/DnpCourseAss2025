using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using RepositoryContracts;

namespace EfcRepositories;

public class EfcCommentRepository : ICommentRepository
{
    private readonly AppContext ctx;

    public EfcCommentRepository(AppContext ctx)
    {
        this.ctx = ctx;
    }

    public async Task<Comment> AddAsync(Comment comment)
    {
        EntityEntry<Comment> entityEntry = await ctx.Comments.AddAsync(comment);
        await ctx.SaveChangesAsync();
        return entityEntry.Entity;
    }

    public async Task UpdateAsync(Comment comment)
    {
        if (!(await ctx.Comments.AnyAsync(c => c.Id == comment.Id)))
        {
            throw new InvalidOperationException(
                $"Kommentar med id {comment.Id} ikke fundet !");
        }

        ctx.Comments.Update(comment);
        await ctx.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        Comment? existing =
            await ctx.Comments.SingleOrDefaultAsync(c => c.Id == id);
        if (existing == null)
        {
            throw new InvalidOperationException(
                $"Kommentar med id {id} ikke fundet !");
        }

        ctx.Comments.Remove(existing);
        await ctx.SaveChangesAsync();
    }

    public async Task<Comment> GetSingleAsync(int id)
    {
        Comment? existing =
            await ctx.Comments.SingleOrDefaultAsync(c => c.Id == id);
        if (existing == null)
        {
            throw new InvalidOperationException(
                $"Kommentar med id {id} ikke fundet !");
        }

        return existing;
    }

    public IQueryable<Comment> GetMany()
    {
        return ctx.Comments.AsQueryable();
    }
}