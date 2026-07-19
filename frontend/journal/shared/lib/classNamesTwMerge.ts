import { clsx, type ClassValue } from "clsx"
import { twMerge } from "tailwind-merge"

export const classNamesTwMerge = (...classes: ClassValue[]) => {
	return twMerge(clsx(classes))
}
