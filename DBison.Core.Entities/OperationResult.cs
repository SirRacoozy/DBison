using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBison.Core.Entities;
public class OperationResult
{
    #region - properties -

    #region [Succeeded]
    /// <summary>
    /// Gets or sets the succeeded flag.
    /// </summary>
    public bool Succeeded { get; set; } = false;
    #endregion

    #region [Message]
    /// <summary>
    /// Gets or sets the message. Could be null.
    /// </summary>
    public string? Message { get; set; }
    #endregion

    #region [Exception]
    /// <summary>
    /// Gets or sets the exception.
    /// </summary>
    public Exception? Exception { get; set; }
    #endregion

    #endregion

    #region - methods -

    #region [Ok]
    /// <summary>
    /// Creating a succeeded operation result.
    /// </summary>
    /// <param name="message">A message. Optional.</param>
    /// <returns>The succeeded operation result.</returns>
    public static OperationResult Ok(string? message = null) => new() { Succeeded = true, Message = message };
    #endregion

    #region [Fail]
    /// <summary>
    /// Creating a failed operation result.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="ex">The exception. Optional.</param>
    /// <returns>The failed operation result.</returns>
    public static OperationResult Fail(string message, Exception? ex = null) => new() { Succeeded = false, Message = message, Exception = ex };
    #endregion

    #endregion
}

public class OperationResult<T> : OperationResult
{
    #region - properties -

    #region [Result]
    /// <summary>
    /// Gets or sets the result.
    /// </summary>
    public T? Result { get; set; }
    #endregion

    #endregion

    #region - methods -

    #region [Ok]
    /// <summary>
    /// Creating a succeeded operation result.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <param name="message">The message. Optional.</param>
    /// <returns>The succeeded operation result.</returns>
    public static OperationResult<T> Ok(T result, string? message = null) => new() { Succeeded = true, Result = result, Message = message };
    #endregion

    #region [Fail]
    // <summary>
    /// Creating a failed operation result.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="ex">The exception. Optional.</param>
    /// <returns>The failed operation result.</returns>
    public static new OperationResult<T> Fail(string message, Exception? ex = null) => new() { Succeeded = false, Result = default, Message = message, Exception = ex };
    #endregion

    #endregion
}

