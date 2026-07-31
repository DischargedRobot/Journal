import { memo, ReactNode } from "react"

const Label = ({
	className,
	children,
}: {
	className?: string
	children: ReactNode
}) => {
	return <span className={className}>{children}</span>
}
export default memo(Label)
