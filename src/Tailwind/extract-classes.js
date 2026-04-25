import fs from 'fs';
import path from 'path';
import { globSync } from 'glob';

const classRegex = /class'\s*=\s*"([^"]+)"/g;
const projectRoot = path.resolve(import.meta.url, '../EvgTsvDotMe');

// Find all F# files
const fsFiles = globSync('../EvgTsvDotMe/**/*.fs', { 
  cwd: import.meta.dirname,
  absolute: true 
});

let allClasses = new Set();

console.log(`Found ${fsFiles.length} F# files`);

// Extract classes from each file
fsFiles.forEach(file => {
  const content = fs.readFileSync(file, 'utf8');
  let match;
  while ((match = classRegex.exec(content)) !== null) {
    const classes = match[1].split(/\s+/).filter(c => c.length > 0);
    classes.forEach(cls => {
      allClasses.add(cls);
    });
  }
});

console.log(`Found ${allClasses.size} unique classes:`);
console.log(Array.from(allClasses).sort().join(' '));

// Create a dummy HTML file with all classes for Tailwind to scan
const dummyHtml = `<!-- Auto-generated file for Tailwind CSS scanning -->
<!DOCTYPE html>
<html>
<body>
<div class="${Array.from(allClasses).join(' ')}"></div>
</body>
</html>`;

const outputFile = path.join(import.meta.dirname, 'classes.html');
fs.writeFileSync(outputFile, dummyHtml);
console.log(`\nCreated ${outputFile}`);

