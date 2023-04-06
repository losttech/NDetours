namespace NDetours;

using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

class DebugTextWriter: TextWriter {
    readonly TextWriter inner;

    public DebugTextWriter(TextWriter inner) {
        this.inner = inner ?? throw new ArgumentNullException(nameof(inner));
    }

    public override void Close() => this.inner.Close();

    protected override void Dispose(bool disposing) {
        if (disposing) this.inner.Dispose();
    }

    public override void Flush() {
        this.inner.Flush();
    }

    public override Task FlushAsync() {
        return this.inner.FlushAsync();
    }

    public override void Write(bool value) {
        Debug.Write(value);
        this.inner.Write(value);
    }

    public override void Write(char value) {
        Debug.Write(value);
        this.inner.Write(value);
    }

    public override void Write(char[]? buffer) {
        Debug.Write(new string(buffer));
        this.inner.Write(buffer);
    }

    public override void Write(char[] buffer, int index, int count) {
        Debug.Write(new string(buffer, index, length: count));
        this.inner.Write(buffer, index, count);
    }

    public override void Write(decimal value) {
        Debug.Write(value);
        this.inner.Write(value);
    }

    public override void Write(double value) {
        Debug.Write(value);
        this.inner.Write(value);
    }

    public override void Write(int value) {
        Debug.Write(value);
        this.inner.Write(value);
    }

    public override void Write(long value) {
        Debug.Write(value);
        this.inner.Write(value);
    }

    public override void Write(object? value) {
        Debug.Write(value);
        this.inner.Write(value);
    }

    public override void Write(float value) {
        Debug.Write(value);
        this.inner.Write(value);
    }

    public override void Write(string? value) {
        Debug.Write(value);
        this.inner.Write(value);
    }

    public override void Write(string format, object? arg0) {
        Debug.Write(string.Format(this.FormatProvider, format, arg0));
        this.inner.Write(format, arg0);
    }

    public override void Write(string format, object? arg0, object? arg1) {
        Debug.Write(string.Format(this.FormatProvider, format, arg0, arg1));
        this.inner.Write(format, arg0, arg1);
    }

    public override void Write(string format, object? arg0, object? arg1, object? arg2) {
        Debug.Write(string.Format(this.FormatProvider, format, arg0, arg1, arg2));
        this.inner.Write(format, arg0, arg1, arg2);
    }

    public override void Write(string format, params object?[] arg) {
        Debug.Write(string.Format(this.FormatProvider, format, arg));
        this.inner.Write(format, arg);
    }

    public override void Write(uint value) {
        Debug.Write(value);
        this.inner.Write(value);
    }

    public override void Write(ulong value) {
        Debug.Write(value);
        this.inner.Write(value);
    }

    public override Task WriteAsync(char value) {
        Debug.Write(value);
        return this.inner.WriteAsync(value);
    }

    // public override Task WriteAsync(char[] buffer) {
    //     return this.inner.WriteAsync(buffer);
    // }

    public override Task WriteAsync(char[] buffer, int index, int count) {
        Debug.Write(new string(buffer, index, length: count));
        return this.inner.WriteAsync(buffer, index, count);
    }

    public override Task WriteAsync(string? value) {
        Debug.Write(value);
        return this.inner.WriteAsync(value);
    }

    public override void WriteLine() {
        Debug.WriteLine("");
        this.inner.WriteLine();
    }

    public override void WriteLine(bool value) {
        Debug.WriteLine(value);
        this.inner.WriteLine(value);
    }

    public override void WriteLine(char value) {
        Debug.WriteLine(value);
        this.inner.WriteLine(value);
    }

    public override void WriteLine(char[]? buffer) {
        Debug.WriteLine(new string(buffer));
        this.inner.WriteLine(buffer);
    }

    public override void WriteLine(char[] buffer, int index, int count) {
        Debug.WriteLine(new string(buffer, index, count));
        this.inner.WriteLine(buffer, index, count);
    }

    public override void WriteLine(decimal value) {
        Debug.WriteLine(value);
        this.inner.WriteLine(value);
    }

    public override void WriteLine(double value) {
        Debug.WriteLine(value);
        this.inner.WriteLine(value);
    }

    public override void WriteLine(int value) {
        Debug.WriteLine(value);
        this.inner.WriteLine(value);
    }

    public override void WriteLine(long value) {
        Debug.WriteLine(value);
        this.inner.WriteLine(value);
    }

    public override void WriteLine(object? value) {
        Debug.WriteLine(value);
        this.inner.WriteLine(value);
    }

    public override void WriteLine(float value) {
        Debug.WriteLine(value);
        this.inner.WriteLine(value);
    }

    public override void WriteLine(string? value) {
        Debug.WriteLine(value);
        this.inner.WriteLine(value);
    }

    public override void WriteLine(string format, object? arg0) {
        Debug.WriteLine(string.Format(this.FormatProvider, format, arg0));
        this.inner.WriteLine(format, arg0);
    }

    public override void WriteLine(string format, object? arg0, object? arg1) {
        Debug.WriteLine(string.Format(this.FormatProvider, format, arg0, arg1));
        this.inner.WriteLine(format, arg0, arg1);
    }

    public override void WriteLine(string format, object? arg0, object? arg1, object? arg2) {
        Debug.WriteLine(string.Format(this.FormatProvider, format, arg0, arg1, arg2));
        this.inner.WriteLine(format, arg0, arg1, arg2);
    }

    public override void WriteLine(string format, params object?[] arg) {
        Debug.WriteLine(string.Format(this.FormatProvider, format, arg));
        this.inner.WriteLine(format, arg);
    }

    public override void WriteLine(uint value) {
        Debug.WriteLine(value);
        this.inner.WriteLine(value);
    }

    public override void WriteLine(ulong value) {
        Debug.WriteLine(value);
        this.inner.WriteLine(value);
    }

    public override Task WriteLineAsync() {
        Debug.WriteLine("");
        return this.inner.WriteLineAsync();
    }

    public override Task WriteLineAsync(char value) {
        Debug.WriteLine(value);
        return this.inner.WriteLineAsync(value);
    }

    // public Task WriteLineAsync(char[] buffer) {
    //     return this.inner.WriteLineAsync(buffer);
    // }

    public override Task WriteLineAsync(char[] buffer, int index, int count) {
        Debug.WriteLine(new string(buffer, index, length: count));
        return this.inner.WriteLineAsync(buffer, index, count);
    }

    public override Task WriteLineAsync(string? value) {
        Debug.WriteLine(value);
        return this.inner.WriteLineAsync(value);
    }

    public override Encoding Encoding => this.inner.Encoding;
}