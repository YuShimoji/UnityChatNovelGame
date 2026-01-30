import sys
import os

def fix_encoding(file_path):
    try:
        # Try reading as Shift-JIS (CP932)
        with open(file_path, 'r', encoding='cp932') as f:
            content = f.read()
        
        # Check if it looks valid (simple heuristic: no replacement characters if strict? 
        # But we just want to see if it makes sense)
        print(f"Successfully read {file_path} as CP932.")
        
        # Write back as UTF-8 with BOM (to make Unity happy and ensure it's treated as UTF-8)
        with open(file_path, 'w', encoding='utf-8-sig') as f:
            f.write(content)
            
        print(f"Converted {file_path} to UTF-8 with BOM.")
        
    except Exception as e:
        print(f"Error processing {file_path}: {e}")

if __name__ == "__main__":
    if len(sys.argv) < 2:
        print("Usage: python fix_encoding.py <file_path>")
    else:
        fix_encoding(sys.argv[1])
